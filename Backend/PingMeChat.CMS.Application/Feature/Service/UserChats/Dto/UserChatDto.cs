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
        public bool IsMuted { get; set; }
        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRead { get; set; }
        public bool IsOnline { get; set; }
        public bool IsTyping { get; set; }
    // Giải thích chi tiết các trường:
    
    // UserId: Định danh duy nhất của người dùng trong cuộc trò chuyện
    // ChatId: Định danh duy nhất của cuộc trò chuyện
    
    // IsAdmin: Xác định người dùng có quyền quản lý cuộc trò chuyện hay không
    // - True: Người dùng có quyền quản lý (ví dụ: thêm/xóa thành viên, thay đổi cài đặt chat)
    // - False: Người dùng là thành viên bình thường
    
    // IsMuted: Xác định người dùng có bị tắt tiếng trong cuộc trò chuyện hay không
    // - True: Người dùng không thể gửi tin nhắn trong cuộc trò chuyện
    // - False: Người dùng có thể gửi tin nhắn bình thường
    
    // IsBanned: Xác định người dùng có bị cấm khỏi cuộc trò chuyện hay không
    // - True: Người dùng bị cấm, không thể tham gia hoặc xem cuộc trò chuyện
    // - False: Người dùng có quyền tham gia cuộc trò chuyện
    
    // IsDeleted: Xác định người dùng đã bị xóa khỏi cuộc trò chuyện hay chưa
    // - True: Người dùng đã bị xóa khỏi cuộc trò chuyện
    // - False: Người dùng vẫn là thành viên của cuộc trò chuyện
    
    // IsRead: Xác định người dùng đã đọc tin nhắn mới nhất trong cuộc trò chuyện hay chưa
    // - True: Người dùng đã đọc tất cả tin nhắn trong cuộc trò chuyện
    // - False: Có tin nhắn mới chưa đọc
    
    // IsOnline: Xác định trạng thái trực tuyến của người dùng trong cuộc trò chuyện
    // - True: Người dùng đang online và có thể nhận tin nhắn ngay lập tức
    // - False: Người dùng đang offline
    
    // IsTyping: Xác định người dùng có đang nhập tin nhắn hay không
    // - True: Người dùng đang nhập tin nhắn
    // - False: Người dùng không đang nhập tin nhắn
    
    // Các trường này giúp quản lý trạng thái và quyền hạn của người dùng trong cuộc trò chuyện,
    // cũng như cung cấp thông tin về hoạt động của họ để tạo trải nghiệm người dùng tốt hơn.
    }
}
