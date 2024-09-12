using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Service.CallParticipants.Dto;
using PingMeChat.CMS.Entities.Feature;
// 

namespace PingMeChat.CMS.Application.Feature.Service.Calls.Dto
{
    public class CallCreateDto : CreateDto
    {
        public string CallerId { get; set; }
        public string ChatId { get; set; }
        public CallType CallType { get; set; }
        public ICollection<CallParticipantCreateDto> CallParticipants { get; set; }
    }
}
