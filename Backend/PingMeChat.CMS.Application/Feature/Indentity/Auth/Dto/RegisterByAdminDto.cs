using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto
{
    public class RegisterByAdminDto
    {
        public string Email { get; set; }
        public string PositionCode { get; set; } // mã nhân viên hoặc mã sinh viên
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public PositionType Position { get; set; }
    }
}
