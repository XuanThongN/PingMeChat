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

namespace PingMeChat.CMS.Application.Feature.Services.RabbitMQServices
{
    public class MessageProcessingService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public MessageProcessingService(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                Port = 5672,
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;
            _channel.QueueDeclare(queue: "chat_messages", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var messageDto = JsonSerializer.Deserialize<MessageCreateDto>(json);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var messageProcessor = scope.ServiceProvider.GetRequiredService<MessageProcessor>();
                    await messageProcessor.ProcessMessageAsync(messageDto);
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: "chat_messages", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}