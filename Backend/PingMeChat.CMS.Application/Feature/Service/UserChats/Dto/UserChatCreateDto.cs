using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.UserChats.Dto
{
    public class UserChatCreateDto : CreateDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ChatId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
