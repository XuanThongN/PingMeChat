using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;
using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Contacts;
using PingMeChat.CMS.Application.Feature.Service.Messages;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Application.Feature.Service.Notifications.Dto;
using PingMeChat.CMS.Application.Feature.Services;
using PingMeChat.CMS.Application.Feature.Services.RabbitMQServices;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Enum;
using PingMeChat.Shared.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IChatHubService _chatHubService;
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;
        private readonly IContactService _contactService;
        private readonly IRedisConnectionManager _redisConnectionManager; // Service quản lý connect ws của user
        private static readonly SemaphoreSlim _throttler = new SemaphoreSlim(100, 100);
        private static readonly TimeSpan _throttlePeriod = TimeSpan.FromSeconds(1);
        public ChatHub(
            IRabbitMQService rabbitMQService,
            IChatHubService chatHubService,
            IChatService chatService,
            IMessageService messageService,
            IContactService contactService,
            IRedisConnectionManager redisConnectionManager)
        {
            _rabbitMQService = rabbitMQService;
            _chatHubService = chatHubService;
            _chatService = chatService;
            _messageService = messageService;
            _contactService = contactService;
            _redisConnectionManager = redisConnectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User not authenticated");
            }

            await _redisConnectionManager.AddConnectionAsync(userId, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await JoinUserChatsAsync(userId, Context.ConnectionId);

            Console.WriteLine($"User {userId} connected with connection {Context.ConnectionId}");

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                await _redisConnectionManager.RemoveConnectionAsync(userId, Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} disconnected from {Context.ConnectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartNewChatAsync(ChatCreateDto chatCreateDto)
        {
            // Tạo ValidationContext từ DTO
            var context = new ValidationContext(chatCreateDto, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            // Thực hiện kiểm tra validation
            bool isValid = Validator.TryValidateObject(chatCreateDto, context, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                // Ném lỗi nếu không hợp lệ, trả về thông báo lỗi cho người dùng
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                throw new HubException($"Validation failed: {errors}");
            }

            var currentUserId = Context.User.FindFirstValue("UserId");
            var newChat = await _chatService.CreateChatAsync(chatCreateDto, currentUserId);

            var userIds = newChat.UserChats.Select(uc => uc.UserId).ToList();
            var connectionIdDictionaries = await _redisConnectionManager.GetBulkConnectionsAsync(userIds);

            var addToGroupTasks = connectionIdDictionaries
                .SelectMany(dict => dict.Value.Select(
                    connectionId =>
                    {
                        System.Console.WriteLine($"Adding {connectionId} to group {newChat.Id}");
                        return Groups.AddToGroupAsync(connectionId, newChat.Id);
                    }))
                .ToList();

            await Task.WhenAll(addToGroupTasks);

            // Kiểm tra chỉ tạo mới cuộc trò chuyện nhóm thì mới thông báo tới những người thành viên
            if (chatCreateDto.IsGroup)
            {
                Console.WriteLine($"New group chat created: {newChat.Id}");
                await Clients.Groups(newChat.Id).SendAsync("NewGroupChat", newChat);
            }
            else
            {
                await Clients.Caller.SendAsync("NewPrivateChat", newChat);
            }
        }

        public async Task SendMessage(MessageCreateDto messageCreateDto)
        {
            await ThrottleAsync();
            ValidationMessageCreateDto(messageCreateDto);
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User not authenticated");
            }
            if (!await HasChatAccess(messageCreateDto.ChatId))
            {
                throw new AppException("Access denied");
            }
            // Gán người gửi vào dto
            messageCreateDto.SenderId = userId;
            // // Lưu tin nhắn vào database
            // await _messageService.SendMessageAsync(messageCreateDto);

            // Đưa tin nhắn vào hàng đợi RabbitMQ
            _rabbitMQService.PublishMessage("chat_messages", messageCreateDto);

            // Gửi tín hiệu SignalR để cập nhật giao diện người dùng
            var attachments = messageCreateDto.Attachments?.Select(a =>
                            {
                                return new AttachmentDto
                                {
                                    FilePath = a.FileUrl,
                                    FileName = a.FileName,
                                    FileType = FileTypeHelper.GetFileTypeFromMimeType(a.FileType).GetDescription(),
                                    FileSize = a.FileSize
                                };
                            }).ToList();
            var result = new MessageDto
            {
                ChatId = messageCreateDto.ChatId,
                SenderId = userId,
                Content = messageCreateDto.Content!,
                CreatedDate = DateTime.UtcNow,
                Attachments = attachments
            };

            // Gửi tin nhắn (xác nhận tin nhắn đã được gửi) về cho caller (người gửi)
            await Clients.Caller.SendAsync("SentMessage", new { TempId = messageCreateDto.TempId, Message = result });
            // Gửi tin nhắn đến các người dùng khác trong nhóm chat
            await _chatHubService.SendMessageAsync(result, Context.ConnectionId);

            // Đưa thông báo vào hàng đợi RabbitMQ
            var notification = new NotificationDto
            {
                ChatId = messageCreateDto.ChatId,
                SenderId = userId,
                Content = messageCreateDto.Content,
                Metadata = new Dictionary<string, object> { { "message", result } }
            };
            _rabbitMQService.PublishNotification("notification_queue", notification);
        }

        public async Task MarkMessageAsRead(String chatId, string messageId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User not authenticated");
            }
            // Kiểm tra xem người dùng có quyền truy cập tin nhắn hay không
            if (!await HasChatAccess(chatId))
            {
                throw new AppException("Access denied");
            }

            // Publish message read event to RabbitMQ
            _rabbitMQService.PublishMessage("message_read", new { MessageId = messageId, ReaderId = userId, ChatId = chatId });
            // Gửi tin nhắn đến tất cả người dùng trong nhóm chat realtime 
            var messageReader = new MessageReader { MessageId = messageId, ReaderId = userId, ReadAt = DateTime.UtcNow };
            await _chatHubService.MarkMessageAsReadAsync(chatId, messageReader);
        }

        // Kiểm tra xem người dùng có quyền truy cập tin nhắn hay không
        private async Task<bool> HasMessageAccess(string messageId, string userId)
        {
            return await _messageService.HasMessageAccess(messageId, userId);
        }

        private void ValidationMessageCreateDto(MessageCreateDto messageCreateDto)
        {
            var context = new ValidationContext(messageCreateDto, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(messageCreateDto, context, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                throw new AppException($"Validation failed: {errors}");
            }
        }

        public async Task JoinChat(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User not authenticated");
            }
            await _chatHubService.JoinGroupAsync(Context.ConnectionId, chatId);
        }

        // Hiển thị người gõ tin nhắn
        public async Task UserTyping(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId) || !await HasChatAccess(chatId))
            {
                throw new HubException("Access denied");
            }

            await Clients.OthersInGroup(chatId).SendAsync("UserTyping", chatId, userId);
        }

        //Tắt hiển thị người gõ tin nhắn
        public async Task UserStopTyping(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId) || !await HasChatAccess(chatId))
            {
                throw new HubException("Access denied");
            }

            await Clients.OthersInGroup(chatId).SendAsync("UserStopTyping", chatId, userId);
        }

        private async Task<bool> HasChatAccess(string chatId)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            return await _chatService.CanUserAccessChat(chatId, userId);
        }

        private async Task JoinUserChatsAsync(string userId, string connectionId)
        {
            var chatIds = await _chatService.GetChatIdListAsync(userId);
            var joinTasks = chatIds.Select(chatId => Groups.AddToGroupAsync(connectionId, chatId));
            await Task.WhenAll(joinTasks);
            Console.WriteLine($"User {userId} joined {chatIds.Count} chats");
        }

        private async Task SendIsActiveAsync(string userId)
        {
            // Lấy danh sách bạn bè hoặc những người bạn bè
            var frienContactIds = await _contactService.GetAllFriendContactIds(userId);

            // Cập nhật trạng thái hoạt động chỉ tới những người bạn bè
            foreach (var frienContactId in frienContactIds)
            {
                await Clients.User(frienContactId).SendAsync("UserStatusChanged", userId, true);
            }
        }

        private async Task SendIsNotActiveAsync(string userId)
        {
            // Kiểm tra nếu không còn connectionId nào thì cập nhật trạng thái hoạt động
            List<string> connectionIdList = await _redisConnectionManager.GetConnectionsAsync(userId);
            var isActive = connectionIdList.Any();

            // Lấy danh sách bạn bè hoặc những người bạn bè
            var frienContactIds = await _contactService.GetAllFriendContactIds(userId);

            // Cập nhật trạng thái hoạt động chỉ tới những người dùng liên quan
            foreach (var frienContactId in frienContactIds)
            {
                await Clients.User(frienContactId).SendAsync("UserStatusChanged", userId, isActive);
            }
        }


        private async Task ThrottleAsync()
        {
            if (!await _throttler.WaitAsync(TimeSpan.Zero))
            {
                throw new HubException("Too many requests. Please try again later.");
            }

            try
            {
                await Task.Delay(_throttlePeriod);
            }
            finally
            {
                _throttler.Release();
            }
        }
    }

}

