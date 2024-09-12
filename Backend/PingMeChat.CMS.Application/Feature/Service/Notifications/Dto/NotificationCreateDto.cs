using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Notifications.Dto
{
    public class NotificationCreateDto : CreateDto
    {
        [Required(ErrorMessage = "Recipient ID is required")]
        public string RecipientId { get; set; }

        [Required(ErrorMessage = "Notification type is required")]
        public NotificationType Type { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public bool IsRead { get; set; } = false;

        public string? SenderId { get; set; }

        [Required(ErrorMessage = "Target type is required")]
        public string TargetType { get; set; }

        [Required(ErrorMessage = "Target ID is required")]
        public string TargetId { get; set; }

        public Dictionary<string, object> Metadata { get; set; }

        // Chú thích:
        // - RecipientId: ID của người nhận thông báo
        // - Type: Loại thông báo (sử dụng enum NotificationType)
        // - Content: Nội dung thông báo
        // - IsRead: Trạng thái đã đọc (mặc định là false khi tạo mới)
        // - SenderId: ID của người gửi thông báo (có thể null)
        // - TargetType: Loại đối tượng liên quan đến thông báo
        // - TargetId: ID của đối tượng liên quan đến thông báo
        // - Metadata: Thông tin bổ sung liên quan đến thông báo

    }
}
