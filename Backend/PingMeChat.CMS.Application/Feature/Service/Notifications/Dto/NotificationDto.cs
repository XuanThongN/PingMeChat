using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Notifications.Dto
{
    public class NotificationDto : ReadDto
    {
        public string ChatId { get; set; }
        public string SenderId { get; set; }
        public string? Content { get; set; }
        public AccountDto? Sender { get; set; }
        public ChatDto? Chat { get; set; }
        public Dictionary<string, object>? Metadata { get; set; } // Thông tin phụ (metadata) liên quan đến thông báo ví dụ như: tin nhắn, cuộc trò chuyện

    }
    // public class NotificationDto : ReadDto
    // {
    //     // Loại thông báo
    //     public NotificationType Type { get; set; }

    //     // Nội dung thông báo
    //     public string Content { get; set; }

    //     // Trạng thái đã đọc hay chưa
    //     public bool IsRead { get; set; }

    //     // Người gửi thông báo (nếu có)
    //     public UserDto Sender { get; set; }

    //     // Đối tượng liên quan đến thông báo (ví dụ: tin nhắn, cuộc trò chuyện)
    //     public object RelatedObject { get; set; }

    //     // Chú thích:
    //     // - NotificationType: Enum định nghĩa các loại thông báo khác nhau
    //     // - UserDto: Đối tượng chứa thông tin người dùng
    //     // - RelatedObject: Có thể là ChatDto hoặc MessageDto tùy thuộc vào loại thông báo

    //     // Thuộc tính mới
    //     public string TargetType { get; set; } // Loại đối tượng liên quan đến thông báo
    //     public string TargetId { get; set; } // Id của đối tượng liên quan đến thông báo

    //     // Thuộc tính mới
    //     public UserDto Recipient { get; set; } // Người nhận thông báo
    //     public Dictionary<string, object> Metadata { get; set; } // Thông tin phụ (metadata) liên quan đến thông báo ví dụ như: tin nhắn, cuộc trò chuyện
    // }

}
