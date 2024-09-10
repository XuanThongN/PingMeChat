using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto
{
    public class BidaTableSessionCreateDto : CreateDto
    {
        public DateTime StartTime { get; set; } // giờ bắt đầu
        public DateTime? EndTime { get; set; } // giờ  kết thúc 
        public SessionStatus SessionStatus { get; set; }
        public string BidaTableId { get; set; }
    }
}
