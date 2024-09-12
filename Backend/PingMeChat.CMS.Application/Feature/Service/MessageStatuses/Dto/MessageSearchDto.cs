using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.MessageStatuses.Dto
{
    public  class MessageStatusSearchDto : RequestDataTable
    {
        public string? MessageId { get; set; }
        public string? UserId { get; set; }
        public bool? IsRead { get; set; }
    }
}
