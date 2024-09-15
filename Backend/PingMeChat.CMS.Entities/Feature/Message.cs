using Org.BouncyCastle.Asn1.Mozilla;
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
        public string ChatId { get; set; }
        public string SenderId { get; set; }
        public string? Content { get; set; }
        public DateTime SentAt { get; set; }
        public virtual List<Attachment>? Attachments { get; set; } = new List<Attachment>();
        public ICollection<MessageReader>? MessageReaders { get; set; } = new List<MessageReader>();  // Danh sách người đọc tin nhắn
        public virtual Chat Chat { get; set; }
        public virtual Account Sender { get; set; }
    }

    public class MessageReader
    {
        public string MessageId { get; set; }
        public string ReaderId { get; set; }
        public DateTime ReadAt { get; set; }
    }


}
