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
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NotificationProcessingService(
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
            _channel.QueueDeclare(queue: "notification_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine("NotificationProcessingService started");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("ExecuteAsync started");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                Console.WriteLine("Notification received");

                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Notification body: {json}");

                var notificationDto = JsonSerializer.Deserialize<NotificationDto>(json);
                Console.WriteLine($"Deserialized notification: {notificationDto}");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var notificationService = scope.ServiceProvider.GetRequiredService<IFCMService>();
                    var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

                    // Truy xuất danh sách người nhận từ cơ sở dữ liệu
                    var chat = await chatService.Find(x => x.Id == notificationDto!.ChatId,
                                                        include: x => x.Include(c => c.UserChats)
                                                                    .ThenInclude(uc => uc.User));
                    // Xoá người gửi khỏi danh sách người nhận
                    chat.UserChats = chat.UserChats.Where(uc => uc.UserId != notificationDto!.SenderId).ToList();

                    // Gửi thông báo cho từng người nhận
                    foreach (var userReceiver in chat.UserChats)
                    {
                        if (!string.IsNullOrEmpty(userReceiver?.UserDto?.FCMToken))
                        {
                            await notificationService.SendNotificationAsync(
                                    userReceiver.UserDto.FCMToken,
                                    !chat.IsGroup ? userReceiver.UserDto.FullName : userReceiver.UserDto.FullName + " to " + (chat.Name ?? "your group"),
                                    notificationDto.Content ?? $"Sent a new message",
                                    new Dictionary<string, string>
                            {
                                { "ChatId", notificationDto.ChatId },
                                { "SenderId", notificationDto.SenderId }
                            });
                        }
                    }
                    Console.WriteLine("Notification processed");
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                Console.WriteLine("Notification acknowledged");
            };

            _channel.BasicConsume(queue: "notification_queue", autoAck: false, consumer: consumer);

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