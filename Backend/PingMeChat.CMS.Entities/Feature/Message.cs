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
    // Message Entity
    public class Message : AuditableBaseEntity
    {
        public Message()
        {
            Attachments = new HashSet<Attachment>();
        }
        public string ChatId { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } 
        public MessageType MessageType { get; set; }

        public virtual Chat Chat { get; set; }
        public virtual Account Sender { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }

    }

    public enum MessageType
    {
        Text,
        Image,
        Video,
        Audio,
        File
    }


}
