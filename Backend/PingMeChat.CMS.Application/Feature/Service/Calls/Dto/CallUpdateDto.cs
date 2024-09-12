using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.Calls.Dto
{
    public class CallUpdateDto : UpdateDto
    {
        public string? ChatId { get; set; }
        public string? UserId { get; set; }
        public string? CallType { get; set; }
        public string? CallStatus { get; set; }
        public string? CallDuration { get; set; }
        public string? CallStartTime { get; set; }
        public string? CallEndTime { get; set; }
        public string? CallerId { get; set; }
        public string? ReceiverId { get; set; }
    }
}
