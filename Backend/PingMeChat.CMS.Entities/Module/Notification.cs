using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Module
{
    public class Notification : AuditableBaseEntity
    {
        public string Title { get; set; }
        public StatusNotification? Status { get; set; } = StatusNotification.Processing;
        public string Time { get; set; }
        public string Icon { get; set; }
        public string? Content { get; set; }
    }
}
