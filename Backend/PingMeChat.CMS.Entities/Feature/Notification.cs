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
    // Notification entity
    public class Notification : AuditableBaseEntity
    {
        public string? UserId { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public string? Message { get; set; }
        public virtual Account User { get; set; } // Giải thích: Một thông báo chỉ gửi đến một người dùng
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }


}
