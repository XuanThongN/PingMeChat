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
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;
            _channel.QueueDeclare(queue: "chat_messages", durable: true, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine("MessageProcessingService started");

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("ExecuteAsync started");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                Console.WriteLine("Message received");

                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message body: {json}");

                var messageDto = JsonSerializer.Deserialize<MessageCreateDto>(json);
                Console.WriteLine($"Deserialized message: {messageDto}");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var messageProcessor = scope.ServiceProvider.GetRequiredService<MessageProcessor>();
                    await messageProcessor.ProcessMessageAsync(messageDto);
                    Console.WriteLine("Message processed");
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                Console.WriteLine("Message acknowledged");
            };

            _channel.BasicConsume(queue: "chat_messages", autoAck: false, consumer: consumer);

            Console.WriteLine("Consumer is running and listening to the queue");

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