using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace PingMeChat.CMS.Application.Feature.Services.RabbitMQServices
{
    public interface IRabbitMQService
    {
        Task PublishMessageAsync(string queueName, object message);
        Task PublishNotificationAsync(string queueName, object notification);
    }

    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly ConcurrentBag<IModel> _channels;
        private readonly int _maxChannels;
        private readonly Channel<(string queueName, byte[] messageBody)> _messageChannel;
        private readonly int _batchSize;
        private readonly TimeSpan _batchTimeout;

        public RabbitMQService(IConfiguration configuration)
        {
            // Khởi tạo kết nối RabbitMQ
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnection();

            // Cấu hình channel pooling
            _maxChannels = int.Parse(configuration["RabbitMQ:MaxChannels"] ?? "10");
            _channels = new ConcurrentBag<IModel>();

            // Cấu hình batching
            _batchSize = int.Parse(configuration["RabbitMQ:BatchSize"] ?? "100");
            _batchTimeout = TimeSpan.FromSeconds(int.Parse(configuration["RabbitMQ:BatchTimeoutSeconds"] ?? "5"));

            // Tạo channel để buffer tin nhắn
            _messageChannel = Channel.CreateBounded<(string, byte[])>(new BoundedChannelOptions(_batchSize * 10)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

            // Bắt đầu task xử lý batch
            Task.Run(ProcessMessageBatchesAsync);
        }

        // Lấy một channel từ pool hoặc tạo mới nếu cần
        private IModel GetChannel()
        {
            if (_channels.TryTake(out var channel) && channel.IsOpen)
            {
                return channel;
            }

            if (_channels.Count >= _maxChannels)
            {
                throw new InvalidOperationException("Max channel limit reached");
            }

            return _connection.CreateModel();
        }

        // Trả channel về pool hoặc đóng nếu đã đạt giới hạn
        private void ReturnChannel(IModel channel)
        {
            if (channel.IsOpen && _channels.Count < _maxChannels)
            {
                _channels.Add(channel);
            }
            else
            {
                channel.Dispose();
            }
        }

        // Phương thức publish tin nhắn bất đồng bộ
        public async Task PublishMessageAsync(string queueName, object message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            await _messageChannel.Writer.WriteAsync((queueName, body));
        }

        // Phương thức publish thông báo (giống với publish tin nhắn)
        public async Task PublishNotificationAsync(string queueName, object notification)
        {
            await PublishMessageAsync(queueName, notification);
        }

        // Xử lý các batch tin nhắn
        private async Task ProcessMessageBatchesAsync()
        {
            while (true)
            {
                var batch = new List<(string queueName, byte[] messageBody)>();
                var channel = GetChannel();

                try
                {
                    // Đọc tin nhắn vào batch cho đến khi đạt kích thước batch hoặc hết thời gian chờ
                    while (batch.Count < _batchSize)
                    {
                        var item = await _messageChannel.Reader.ReadAsync().AsTask().WaitAsync(_batchTimeout);
                        if (item == default) break;
                        batch.Add(item);
                    }

                    if (batch.Count > 0)
                    {
                        // Gom nhóm tin nhắn theo queue
                        var batchesByQueue = batch.GroupBy(x => x.queueName);
                        foreach (var queueBatch in batchesByQueue)
                        {
                            string queueName = queueBatch.Key;
                            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                            // Publish batch tin nhắn
                            var batchPublish = channel.CreateBasicPublishBatch();
                            foreach (var (_, messageBody) in queueBatch)
                            {
                                batchPublish.Add(exchange: "", routingKey: queueName, mandatory: true, properties: null, body: messageBody);
                            }
                            batchPublish.Publish();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi (có thể log lỗi ở đây)
                    Console.WriteLine($"Error processing message batch: {ex.Message}");
                }
                finally
                {
                    ReturnChannel(channel);
                }
            }
        }

        // Giải phóng tài nguyên
        public void Dispose()
        {
            foreach (var channel in _channels)
            {
                channel.Close();
                channel.Dispose();
            }
            _connection.Close();
            _connection.Dispose();
        }
    }
}