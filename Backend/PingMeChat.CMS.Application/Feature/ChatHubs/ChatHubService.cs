using System;
using Microsoft.AspNetCore.SignalR;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatHubService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageAsync(string chatId, string userId, string message)
        {
            await _hubContext.Clients.Group(chatId).SendAsync("ReceiveMessage", userId, message);
        }

        public async Task JoinGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public async Task SendMessageToGroupAsync(string chatId, string userId, string message)
        {
            await _hubContext.Clients.Group(chatId).SendAsync("ReceiveGroupMessage", userId, message);
        }
    }

}

