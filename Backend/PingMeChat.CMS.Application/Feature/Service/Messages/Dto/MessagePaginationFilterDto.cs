using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Messages.Dto
{
    public class MessagePaginationFilterDto
    {
        public string? ChatId { get; set; }
        public string? SenderId { get; set; }
    }
}
