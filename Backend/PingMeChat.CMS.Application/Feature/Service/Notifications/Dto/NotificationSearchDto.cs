using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Notifications.Dto
{
    public  class NotificationSearchDto : RequestDataTable
    {
        public string? RecipientId { get; set; }
        public string? SenderId { get; set; }
        public NotificationType? Type { get; set; }
        public bool? IsRead { get; set; }
        public string? TargetType { get; set; }
        public string? TargetId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
    }
}
