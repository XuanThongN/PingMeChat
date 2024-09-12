using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Calls.Dto
{
    public class CallSearchDto : RequestDataTable
    {
        public string? ChatId { get; set; }
        public string? UserId { get; set; }
    }
}
