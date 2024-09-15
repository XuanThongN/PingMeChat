using System;
using Microsoft.AspNetCore.SignalR;

namespace PingMeChat.CMS.Application.Feature.ChatHubs;

public class ChatHub : Hub
{
    private readonly IChatHubService _chatHubService;

    public ChatHub(IChatHubService chatHubService)
    {
        _chatHubService = chatHubService;
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
