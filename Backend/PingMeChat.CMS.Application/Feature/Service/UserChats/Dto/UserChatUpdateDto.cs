using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.UserChats.Dto
{
    public class UserChatUpdateDto : UpdateDto
    {
        [Required]
        public string ChatId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        [Required]
        public bool IsMuted { get; set; }
        [Required]
        public bool IsBanned { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsRead { get; set; }
        [Required]
        public bool IsOnline { get; set; }
        [Required]
        public bool IsTyping { get; set; }
    }
}
