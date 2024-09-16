using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using System;
using System.Security.Claims;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatHubService _chatHubService;
        private readonly IChatService _chatService;
        public ChatHub(IChatHubService chatHubService, IChatService chatService)
        {
            _chatHubService = chatHubService;
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.User?.FindFirstValue("UserId");

            if (!string.IsNullOrEmpty(userId))
            {
                // Thêm người dùng vào nhóm cá nhân để hỗ trợ tin nhắn 1-1
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                // Lấy danh sách tất cả các cuộc trò chuyện mà người dùng tham gia (bao gồm cả chat private và chat nhóm)
                var userChats = await _chatService.GetChatListAsync(userId);

                // Thêm người dùng vào tất cả các nhóm chat
                foreach (var chat in userChats)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id);
                }

                // Thông báo cho các người dùng khác về việc người dùng này online
                await Clients.All.SendAsync("ReceiveMessage", $"{userId} has joined");
            }

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left");
        }

        public async Task SendMessage(string chatId, string message)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User not authenticated");
            }
            if (!await HasChatAccess(chatId))
            {
                throw new AppException("Access denied");
            }
            await _chatHubService.SendMessageAsync(chatId, userId, message);
            await _chatHubService.SendMessageAsync(chatId, userId, message);
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

        public async Task SendMessageToGroup(string chatId, string message)
        {
            var userId = Context.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User not authenticated");
            }
            await _chatHubService.SendMessageToGroupAsync(chatId, userId, message);
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
    }

}

