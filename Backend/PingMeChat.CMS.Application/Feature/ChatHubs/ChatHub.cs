using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
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
        private readonly IRedisConnectionManager _redisConnectionManager; // Service quản lý connect ws của user
        private static readonly SemaphoreSlim _throttler = new SemaphoreSlim(100, 100);
        private static readonly TimeSpan _throttlePeriod = TimeSpan.FromSeconds(1);
        public ChatHub(
            IRabbitMQService rabbitMQService,
            IChatHubService chatHubService,
            IChatService chatService,
            IMessageService messageService,
            IRedisConnectionManager redisConnectionManager)
        {
            _rabbitMQService = rabbitMQService;
            _chatHubService = chatHubService;
            _chatService = chatService;
            _messageService = messageService;
            _redisConnectionManager = redisConnectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var userId = httpContext?.User?.FindFirstValue("UserId");

                if (!string.IsNullOrEmpty(userId))
                {
                    await _redisConnectionManager.AddConnectionAsync(userId, Context.ConnectionId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                    await JoinUserChatsAsync(userId, Context.ConnectionId); // Thêm user vào các group 1 cách bất đồng bộ
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error on connected" + ex.Message);
                throw;
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.User?.FindFirstValue("UserId");

            if (!string.IsNullOrEmpty(userId))
            {
                await _redisConnectionManager.RemoveConnectionAsync(userId, Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
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
            // Tạo cuộc trò chuyện mới trong db
            var newChat = await _chatService.CreateChatAsync(chatCreateDto, currentUserId);
            // Thêm các người dùng vào nhóm SignalR với chatId mới
            foreach (var user in newChat.UserChats)
            {
                (await _redisConnectionManager.GetConnectionsAsync(user.UserId))?.ToList()
                     .ForEach(async connectionId =>
                         {
                             await Groups.AddToGroupAsync(connectionId, newChat.Id);
                         });
            }

            // Kiểm tra chỉ tạo mới cuộc trò chuyện nhóm thì mới thông báo tới những người thành viên
            if (chatCreateDto.IsGroup)
            {
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
            _rabbitMQService.PublishMessageAsync("chat_messages", messageCreateDto);

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
            await _chatHubService.SendMessageAsync(result);

            // Đưa thông báo vào hàng đợi RabbitMQ
            var notification = new NotificationDto
            {
                ChatId = messageCreateDto.ChatId,
                SenderId = userId,
                Content = messageCreateDto.Content,
                Metadata = new Dictionary<string, object> { { "message", result } }
            };
            await _rabbitMQService.PublishNotificationAsync("notification_queue", notification);
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
            try
            {
                var userChats = await _chatService.GetChatListAsync(userId);
                if (userChats != null && userChats.Any())
                {
                    Console.WriteLine($"User {userId} is joining {userChats.Count()} chats.");
                    var joinTasks = userChats.Select(chat =>
                    {
                        Console.WriteLine($"User {userId} is joining chat {chat.Id} with connection {connectionId}");
                        return Groups.AddToGroupAsync(connectionId, chat.Id);
                    });

                    await Task.WhenAll(joinTasks);  // Chạy song song việc thêm vào các nhóm
                    Console.WriteLine($"User {userId} has successfully joined all chats.");
                }
                else
                {
                    Console.WriteLine($"User {userId} has no chats to join.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining user {userId} to chats: {ex.Message}");
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

