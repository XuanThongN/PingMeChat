using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.MessageStatuses.Dto
{
    public class MessageStatusDto : ReadDto
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; }
        public AccountDto User { get; set; }
    }
    
}
