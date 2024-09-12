using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Application.Feature.Service.UserChats.Dto;

namespace PingMeChat.CMS.Application.Feature.Service.Chats.Dto
{
    public class ChatDto : ReadDto
    {
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public string? AvatarUrl { get; set; }
        public IEnumerable<UserChatDto> UserChats { get; set; }
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}
