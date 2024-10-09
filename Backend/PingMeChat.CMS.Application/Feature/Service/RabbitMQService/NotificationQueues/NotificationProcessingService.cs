using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using PingMeChat.CMS.Application.Feature.Service.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PingMeChat.CMS.Application.Feature.Service.Notifications.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using Microsoft.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.Feature.Services.RabbitMQServices.NotificationQueues
{
    public class NotificationProcessingService : BackgroundService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _concurrentConsumers;
        private readonly int _prefetchCount;

        public NotificationProcessingService(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _serviceScopeFactory = serviceScopeFactory;
            _concurrentConsumers = int.Parse(configuration["RabbitMQ:ConcurrentConsumers"] ?? "4");
            _prefetchCount = int.Parse(configuration["RabbitMQ:PrefetchCount"] ?? "10");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var tasks = new List<Task>();

            for (int i = 0; i < _concurrentConsumers; i++)
            {
                tasks.Add(StartConsumerAsync(connection, stoppingToken));
            }

            await Task.WhenAll(tasks);
        }

        private async Task StartConsumerAsync(IConnection connection, CancellationToken stoppingToken)
        {
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "notification_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, (ushort)_prefetchCount, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var notificationDto = JsonSerializer.Deserialize<NotificationDto>(json);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var notificationService = scope.ServiceProvider.GetRequiredService<IFCMService>();
                    var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

                    // Xử lý notification như trước đây

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(queue: "notification_queue", autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}