using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.MessageStatuses.Dto
{
    public class MessageStatusCreateDto : CreateDto
    {
        // ID của tin nhắn, bắt buộc phải có
        [Required(ErrorMessage = "Message ID is required")]
        public string MessageId { get; set; }

        // ID của người dùng, bắt buộc phải có
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        // Trạng thái đã đọc hay chưa, mặc định là false (chưa đọc)
        public bool IsRead { get; set; } = false;

        // Chú thích:
        // - MessageId: Dùng để liên kết với tin nhắn cụ thể
        // - UserId: Xác định người dùng liên quan đến trạng thái tin nhắn
        // - IsRead: Cho biết tin nhắn đã được đọc chưa
    }
}
