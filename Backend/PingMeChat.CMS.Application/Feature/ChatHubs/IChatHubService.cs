using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using System;

namespace PingMeChat.CMS.Application.Feature.ChatHubs;

public interface IChatHubService
{
    Task SendMessageAsync(MessageDto chatDto, string callerConnectionId);
    Task JoinGroupAsync(string connectionId, string groupName);

}
