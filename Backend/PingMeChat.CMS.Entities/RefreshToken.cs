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
    public class RefreshToken : AuditableBaseEntity
    {
        public RefreshToken()
        {
            UserSessions = new HashSet<UserSession>();
        }
        public string TokenValue { get; set; }
        public string DeviceInfo { get; set; }
        public string IPAddress { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } // vô hiệu hóa refresh token mà k phải xóa
        public string AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        public virtual ICollection<UserSession> UserSessions { get; set; }
    }
}
