using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.MessageStatuses.Dto;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Messages.Dto
{
    public class MessageDto : ReadDto
    {
        public string ChatId { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsRead { get; set; }
        public AccountDto Sender { get; set; }
        public ChatDto Chat { get; set; }
        public List<MessageStatusDto>? MessageStatuses { get; set; } // danh sách trạng thái đọc tin nhắn
    }
    
}
