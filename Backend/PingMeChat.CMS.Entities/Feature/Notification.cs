using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.CMS.Entities.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // Notification entity
    public class Notification : AuditableBaseEntity
    {
        public string RecipientId { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public string? SenderId { get; set; }
        public string TargetType { get; set; }
        public string TargetId { get; set; }
        public string Metadata { get; set; }

        [ForeignKey("RecipientId")]
        public virtual Account Recipient { get; set; }

        [ForeignKey("SenderId")]
        public virtual Account Sender { get; set; }
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        NewMessage,
        FriendRequest,
        SystemNotification
        // Thêm các loại thông báo khác nếu cần
    }
}
