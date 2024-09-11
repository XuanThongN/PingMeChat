using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.CMS.Entities.Users;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // Attachments entity
    public class Attachment : AuditableBaseEntity
    {
        public string MessageId { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public long FileSize { get; set; }
        public virtual Message Message { get; set; }
    }

}
