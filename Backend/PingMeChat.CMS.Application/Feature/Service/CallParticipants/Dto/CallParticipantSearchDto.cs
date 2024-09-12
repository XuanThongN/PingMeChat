using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.CallParticipants.Dto
{
    public class CallParticipantSearchDto : RequestDataTable
    {
        public string? CallId { get; set; }
    }

}
