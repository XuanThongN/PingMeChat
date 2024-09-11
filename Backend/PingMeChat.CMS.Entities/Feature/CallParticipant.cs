using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // CallParticipant entity
    public class CallParticipant : AuditableBaseEntity
    {
        public string? UserId { get; set; }
        public string? CallId { get; set; }
        public string? Role { get; set; }
        public DateTime JoinedAt { get; set; }  
        public DateTime? LeftAt { get; set; }
        public virtual Account User { get; set; }
        public virtual Call Call { get; set; }
    }


}
