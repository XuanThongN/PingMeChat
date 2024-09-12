using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.MessageStatuses.Dto
{
    public class MessageStatusUpdateDto : UpdateDto
    {
        // ID của tin nhắn, bắt buộc phải có
        [Required(ErrorMessage = "Message ID is required")]
        public string MessageId { get; set; }

        // ID của người dùng, bắt buộc phải có
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        // Trạng thái đã đọc hay chưa
        public bool IsRead { get; set; }

        // Chú thích:
        // - MessageId: Dùng để xác định tin nhắn cần cập nhật trạng thái
        // - UserId: Xác định người dùng liên quan đến việc cập nhật trạng thái tin nhắn
        // - IsRead: Cho phép cập nhật trạng thái đã đọc của tin nhắn
    }
}
