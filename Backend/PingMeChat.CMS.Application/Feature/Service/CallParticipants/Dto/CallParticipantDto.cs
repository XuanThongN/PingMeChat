using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.Shared.Enum;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.CallParticipants.Dto
{
    public class CallParticipantDto : ReadDto
    {
        public string CallId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public bool IsInitiator { get; set; }
        public DateTime JoinedTime { get; set; }
        public DateTime? LeftTime { get; set; }
    }
}
