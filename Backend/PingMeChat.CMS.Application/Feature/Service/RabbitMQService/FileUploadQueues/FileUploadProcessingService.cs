using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using PingMeChat.CMS.Application.Feature.Service.Messages;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HelpMate.Core.Extensions;
using PingMeChat.CMS.Application.Feature.Service.Attachments;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using Microsoft.AspNetCore.Http;
using PingMeChat.CMS.Application.Feature.ChatHubs;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.Shared.Enum;

namespace PingMeChat.CMS.Application.Feature.Services.RabbitMQServices.FileUploadQueues
{
    public class FileUploadProcessingService : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public FileUploadProcessingService(IConfiguration configuration,
           IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare(queue: "file_upload_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var fileUploadMessage = JsonSerializer.Deserialize<FileUploadMessage>(message);

                if (fileUploadMessage != null)
                {
                    await ProcessFileUpload(fileUploadMessage);
                }
            };

            _channel.BasicConsume(queue: "file_upload_queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessFileUpload(FileUploadMessage message)
        {
            try
            {
                CloudinaryUploadResult uploadResult;
                await using (var fileStream = new FileStream(message.FilePath, FileMode.Open, FileAccess.Read))
                {
                    var formFile = new FormFile(fileStream, 0, new FileInfo(message.FilePath).Length, message.FileName, message.FileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = message.MimeType
                    };

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var fileUploadProcessor = scope.ServiceProvider.GetRequiredService<FileUploadProcessor>();

                        // Xử ly upload file và gửi thông báo tới client
                        await fileUploadProcessor.ProcessUploadFileAsync(formFile, message);
                    }
                }

                // Schedule file deletion after 15 minutes
                var timer = new System.Timers.Timer(15 * 60 * 1000);
                timer.Elapsed += (sender, e) =>
                {
                    File.Delete(message.FilePath);
                    timer.Dispose();
                };
                timer.AutoReset = false;
                timer.Start();
            }
            catch (Exception ex)
            {
                throw new AppException("Error occurred while processing file upload", ex);
            }
        }
    }

    public class FileUploadMessage
    {
        public string? UploadId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public long FileSize { get; set; }
        public string? ChatId { get; set; }
        public string? MessageId { get; set; }
    }
}