using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.UserChats.Dto
{
    public class UserChatCreateDto : CreateDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ChatId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsMuted { get; set; }
        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRead { get; set; }
        public bool IsOnline { get; set; }
        public bool IsTyping { get; set; }
        // Giải thích các trường:
        // IsAdmin: Người dùng có quyền quản lý chat hay không
        // IsMuted: Người dùng có bị tống động hay không
        // IsBanned: Người dùng có bị cấm hay không
        // IsDeleted: Người dùng có bị xóa hay không
        // IsRead: Người dùng có đọc tin nhắn hay không
        // IsOnline: Người dùng có online hay không
        // IsTyping: Người dùng có đang gõ tin nhắn hay không
    }
}
