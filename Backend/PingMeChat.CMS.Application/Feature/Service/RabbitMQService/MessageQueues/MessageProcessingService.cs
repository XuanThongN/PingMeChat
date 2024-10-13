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
            // _channel.QueueDeclare(queue: "chat_messages", durable: true, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine("MessageProcessingService started");

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("ExecuteAsync started");

            ConsumeMessages("chat_messages", ProcessChatMessage);
            ConsumeMessages("message_read", ProcessMessageRead);

            return Task.CompletedTask;
        }
        private void ConsumeMessages(string queueName, Func<string, Task> processMessage)
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                try
                {
                    await processMessage(json);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        private async Task ProcessChatMessage(string json)
        {
            var messageDto = JsonSerializer.Deserialize<MessageCreateDto>(json);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var messageProcessor = scope.ServiceProvider.GetRequiredService<MessageProcessor>();
                await messageProcessor.ProcessMessageAsync(messageDto);
            }
        }

        private async Task ProcessMessageRead(string json)
        {
            var messageReadEvent = JsonSerializer.Deserialize<MessageReadEvent>(json);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var messageProcessor = scope.ServiceProvider.GetRequiredService<MessageProcessor>();
                await messageProcessor.ProcessMessageReadAsync(messageReadEvent);
            }
        }
        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}