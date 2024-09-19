using System;

namespace PingMeChat.CMS.Application.Feature.ChatHubs;

public interface IChatHubService
{
    Task SendMessageAsync(string chatId, string senderId, string message);
    Task JoinGroupAsync(string connectionId, string groupName);
    Task SendMessageToGroupAsync(string chatId, string userId, string message);
}
