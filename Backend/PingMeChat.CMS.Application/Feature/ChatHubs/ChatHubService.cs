using System;
using Microsoft.AspNetCore.SignalR;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatHubService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageAsync(MessageDto messageDto, string callerConnectionId)
        {
            // Gửi tin nhắn đến tất cả các người dùng trong nhóm chat trừ người gửi
            await _hubContext.Clients
                    .GroupExcept(messageDto.ChatId, new[] { callerConnectionId })
                    .SendAsync("ReceiveMessage", messageDto);
        }

        public async Task MarkMessageAsReadAsync(string chatId, MessageReader messageReader)
        {
            await _hubContext.Clients.Group(chatId).SendAsync("MessageMarkedAsRead", new { chatId, messageReader });
        }

        public async Task JoinGroupAsync(string connectionId, string groupName)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        }

        public async Task NotifyUploadFileSuccessAsync(AttachmentDto attachmentDto)
        {
            await _hubContext.Clients.Group(attachmentDto.ChatId).SendAsync("UploadFileSuccess", attachmentDto);
        }
    }

}

