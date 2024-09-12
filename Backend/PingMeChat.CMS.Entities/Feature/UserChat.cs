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
    // UserChat entity
    public class UserChat : AuditableBaseEntity
    {
        public string UserId { get; set; }
        public string ChatId { get; set; }
        public bool IsAdmin { get; set; } // IsAdmin
        public DateTime JoinAt { get; set; }   
        public string? Settings { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual Account User { get; set; }
    }


}
