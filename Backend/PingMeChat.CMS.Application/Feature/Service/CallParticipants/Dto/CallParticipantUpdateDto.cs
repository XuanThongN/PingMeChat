using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.CallParticipants.Dto
{
    public class CallParticipantUpdateDto : UpdateDto
    {
        public string? CallId { get; set; }
        public string? UserId { get; set; }
        public bool IsInitiator { get; set; }
        public DateTime JoinedTime { get; set; }
        public DateTime? LeftTime { get; set; }
    }
}
