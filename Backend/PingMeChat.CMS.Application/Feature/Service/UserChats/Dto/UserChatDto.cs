using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.Shared.Enum;

namespace PingMeChat.CMS.Application.Feature.Service.UserChats.Dto
{
    public class UserChatDto : ReadDto
    {
        public string UserId { get; set; }
        public string ChatId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
