using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.Chats.Dto
{
    public class ChatUpdateDto : UpdateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsGroup { get; set; }
        public string? AvatarUrl { get; set; }
        [Required]
        public IEnumerable<string> UserChatIds { get; set; }
    }
}
