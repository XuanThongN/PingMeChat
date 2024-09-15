using System;

namespace PingMeChat.CMS.Application.Feature.ChatHubs;

public interface IChatHubService
{
    Task SendMessageAsync(string userId, string message);
    Task JoinGroupAsync(string connectionId, string groupName);
    Task SendMessageToGroupAsync(string groupName, string message);
}
