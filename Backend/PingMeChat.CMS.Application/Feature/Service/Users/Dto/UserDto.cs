using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Users.Dto
{
    public class UserDto : ReadDto
    {
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime? DateRegistered { get; set; } = DateTime.Now;
        public bool IsLocked { get; set; }
        public string? FullName { get; set; } // tên chủ sở hữu
        public List<string>? GroupIds { get; set; }
        public string? AvatarUrl { get; set; } // đường dẫn ảnh đại diện

    }
}
