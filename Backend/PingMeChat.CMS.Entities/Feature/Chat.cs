using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // Chat entity
    public class Chat : AuditableBaseEntity
    {
        public Chat()
        {
            UserChats = new HashSet<UserChat>();
            Messages = new HashSet<Message>();
        }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public string? AvatarUrl { get; set; }
        public virtual IEnumerable<UserChat> UserChats { get; set; }
        public virtual IEnumerable<Message>? Messages { get; set; }
    }

   
}
