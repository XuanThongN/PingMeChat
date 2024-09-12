using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Chats.Dto
{
    public class ChatCreateDto : CreateDto
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
