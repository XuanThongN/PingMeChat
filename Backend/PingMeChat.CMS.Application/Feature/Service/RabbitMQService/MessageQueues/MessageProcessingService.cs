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

namespace PingMeChat.CMS.Application.Feature.Services.RabbitMQServices.MessageQueues
{
    public class MessageProcessingService : BackgroundService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _concurrentConsumers;
        private readonly int _prefetchCount;

        public MessageProcessingService(
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
            channel.QueueDeclare(queue: "chat_messages", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, (ushort)_prefetchCount, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var messageDto = JsonSerializer.Deserialize<MessageCreateDto>(json);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var messageProcessor = scope.ServiceProvider.GetRequiredService<MessageProcessor>();
                    await messageProcessor.ProcessMessageAsync(messageDto);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(queue: "chat_messages", autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}