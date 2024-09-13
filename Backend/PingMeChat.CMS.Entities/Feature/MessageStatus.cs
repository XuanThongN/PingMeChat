using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // MessageStatus entity
    public class MessageStatus : AuditableBaseEntity
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; } = true;
        public DateTime? ReadAt { get; set; } // Giải thích: Thời gian mà người dùng đã đọc tin nhắn
        public virtual Message Message { get; set; }
        public virtual Account User { get; set; }
    }

}
