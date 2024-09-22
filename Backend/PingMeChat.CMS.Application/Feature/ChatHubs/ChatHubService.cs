using System;
using Microsoft.AspNetCore.SignalR;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatHubService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageAsync(MessageDto messageDto)
        {
            await _hubContext.Clients.Group(messageDto.ChatId).SendAsync("ReceiveMessage", messageDto);
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

