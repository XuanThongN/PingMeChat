using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Service.SessionServices.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto
{
    public class BidaTableSessionDto : ReadDto
    {
        public DateTime StartTime { get; set; } // giờ bắt đầu
        public DateTime? EndTime { get; set; } // giờ  kết thúc 
        public SessionStatus SessionStatus { get; set; }
        public string BidaTableId { get; set; }
        public virtual IEnumerable<SessionServiceDto>? SessionServices { get; set; }

    }
}
