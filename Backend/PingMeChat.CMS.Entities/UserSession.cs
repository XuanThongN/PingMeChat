using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PingMeChat.Shared.Utils.Message.Error;

namespace PingMeChat.CMS.Entities
{
    public class UserSession : AuditableBaseEntity
    {
        public string AccountId { get; set; }
        public string RefreshTokenId { get; set; }
        public string AccessToken { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LastActivityTime { get; set; } // thời gian hoạt động cuối dùng
        public DateTime? LogoutTime { get; set; }
        [ForeignKey("AccountId")]
        public Account Account { get; set; }
        [ForeignKey("RefreshTokenId")]
        public RefreshToken RefreshToken { get; set; }
    }
}
