using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using PingMeChat.CMS.Application.Feature.ChatHubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Services.SignalR
{
    using Microsoft.AspNetCore.SignalR;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    namespace PingMeChat.CMS.Application.Feature.Services.SignalR
    {
        public class RabbitMQBackplane : HubLifetimeManager<ChatHub>
        {
            private readonly IConnection _connection;
            private readonly IModel _channel;
            private const string ExchangeName = "signalr_backplane";

            public RabbitMQBackplane(IConfiguration configuration)
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:HostName"],
                    UserName = configuration["RabbitMQ:UserName"],
                    Password = configuration["RabbitMQ:Password"]
                };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);

                var queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queueName, ExchangeName, "");

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var backplaneMessage = JsonSerializer.Deserialize<BackplaneMessage>(message);
                    ProcessMessage(backplaneMessage);
                };
                _channel.BasicConsume(queueName, true, consumer);
            }

            public override Task OnConnectedAsync(HubConnectionContext connection)
            {
                return Task.CompletedTask;
            }

            public override Task OnDisconnectedAsync(HubConnectionContext connection)
            {
                return Task.CompletedTask;
            }

            public override Task SendAllAsync(string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendAll",
                    MethodName = methodName,
                    Args = args
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task SendAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendAllExcept",
                    MethodName = methodName,
                    Args = args,
                    ExcludedConnectionIds = excludedConnectionIds
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task SendConnectionAsync(string connectionId, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendConnection",
                    ConnectionId = connectionId,
                    MethodName = methodName,
                    Args = args
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendConnections",
                    ConnectionIds = connectionIds,
                    MethodName = methodName,
                    Args = args
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task SendGroupAsync(string groupName, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendGroup",
                    GroupName = groupName,
                    MethodName = methodName,
                    Args = args
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task SendGroupExceptAsync(string groupName, string methodName, object[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendGroupExcept",
                    GroupName = groupName,
                    MethodName = methodName,
                    Args = args,
                    ExcludedConnectionIds = excludedConnectionIds
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendGroups",
                    GroupNames = groupNames,
                    MethodName = methodName,
                    Args = args
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task SendUserAsync(string userId, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendUser",
                    UserId = userId,
                    MethodName = methodName,
                    Args = args
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object[] args, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "SendUsers",
                    UserIds = userIds,
                    MethodName = methodName,
                    Args = args
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "AddToGroup",
                    ConnectionId = connectionId,
                    GroupName = groupName
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            public override Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
            {
                var message = new BackplaneMessage
                {
                    Type = "RemoveFromGroup",
                    ConnectionId = connectionId,
                    GroupName = groupName
                };
                PublishMessage(message);
                return Task.CompletedTask;
            }

            private void PublishMessage(BackplaneMessage message)
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(ExchangeName, "", null, body);
            }

            private void ProcessMessage(BackplaneMessage message)
            {
                switch (message.Type)
                {
                    case "SendAll":
                        SendAllAsync(message.MethodName, message.Args).Wait();
                        break;
                    case "SendAllExcept":
                        SendAllExceptAsync(message.MethodName, message.Args, message.ExcludedConnectionIds).Wait();
                        break;
                    case "SendConnection":
                        SendConnectionAsync(message.ConnectionId, message.MethodName, message.Args).Wait();
                        break;
                    case "SendConnections":
                        SendConnectionsAsync(message.ConnectionIds, message.MethodName, message.Args).Wait();
                        break;
                    case "SendGroup":
                        SendGroupAsync(message.GroupName, message.MethodName, message.Args).Wait();
                        break;
                    case "SendGroupExcept":
                        SendGroupExceptAsync(message.GroupName, message.MethodName, message.Args, message.ExcludedConnectionIds).Wait();
                        break;
                    case "SendGroups":
                        SendGroupsAsync(message.GroupNames, message.MethodName, message.Args).Wait();
                        break;
                    case "SendUser":
                        SendUserAsync(message.UserId, message.MethodName, message.Args).Wait();
                        break;
                    case "SendUsers":
                        SendUsersAsync(message.UserIds, message.MethodName, message.Args).Wait();
                        break;
                    case "AddToGroup":
                        AddToGroupAsync(message.ConnectionId, message.GroupName).Wait();
                        break;
                    case "RemoveFromGroup":
                        RemoveFromGroupAsync(message.ConnectionId, message.GroupName).Wait();
                        break;
                }
            }
        }

        public class BackplaneMessage
        {
            public string Type { get; set; }
            public string GroupName { get; set; }
            public IReadOnlyList<string> GroupNames { get; set; }
            public string MethodName { get; set; }
            public object[] Args { get; set; }
            public string ConnectionId { get; set; }
            public IReadOnlyList<string> ConnectionIds { get; set; }
            public IReadOnlyList<string> ExcludedConnectionIds { get; set; }
            public string UserId { get; set; }
            public IReadOnlyList<string> UserIds { get; set; }
        }
    }
}