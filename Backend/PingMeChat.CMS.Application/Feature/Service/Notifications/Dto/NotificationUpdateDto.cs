using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.Notifications.Dto
{
    public class NotificationUpdateDto : UpdateDto
    {
        public string RecipientId { get; set; }
        public NotificationType Type { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public string? SenderId { get; set; }
        public string TargetType { get; set; }
        public string TargetId { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        
    }
}
