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
    // Contact entity
    public class Contact : AuditableBaseEntity
    {
        public string UserId { get; set; }
        public string ContactUserId { get; set; }
        public string? Nickname { get; set; }
        public DateTime AddedAt { get; set; } 
        public string? Settings { get; set; }
        public virtual Account User { get; set; }
        public virtual Account ContactUser { get; set; }
    }


}
