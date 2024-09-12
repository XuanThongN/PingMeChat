using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.Shared.Enum;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Application.Feature.Service.CallParticipants.Dto;

namespace PingMeChat.CMS.Application.Feature.Service.Calls.Dto
{
    public class CallDto : ReadDto
    {
        public string ChatId { get; set; }
        public CallStatus CallStatus { get; set; }
        public CallType CallType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public ICollection<CallParticipantDto> CallParticipants { get; set; }
    }
}
