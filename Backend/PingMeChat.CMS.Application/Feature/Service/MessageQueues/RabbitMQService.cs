using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PingMeChat.CMS.Application.Feature.Services.RabbitMQServices
{
    public interface IRabbitMQService
    {
        void PublishMessage(string queueName, object message);
    }

    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void PublishMessage(string queueName, object message)
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}