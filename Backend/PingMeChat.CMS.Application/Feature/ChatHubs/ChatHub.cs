using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    public sealed class ChatHub : Hub
    {
        private readonly IChatHubService _chatHubService;
        public ChatHub(IChatHubService chatHubService)
        {
            _chatHubService = chatHubService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext.User.FindFirstValue("UserId");

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left");
        }
        public async Task SendMessage(string userId, string message)
        {
            await _chatHubService.SendMessageAsync(userId, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await _chatHubService.JoinGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            await _chatHubService.SendMessageToGroupAsync(groupName, message);
        }
    }

}

