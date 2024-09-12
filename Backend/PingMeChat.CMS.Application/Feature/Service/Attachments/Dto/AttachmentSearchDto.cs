using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Attachments.Dto
{
    public class AttachmentSearchDto : RequestDataTable
    {
        public string? MessageId { get; set; }
    }

}
