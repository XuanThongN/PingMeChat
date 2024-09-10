using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto
{
    public class BidaTableDto : ReadDto
    {
        public string Code { get; set; }
        public BidaTableStatus BidaTableStatus { get; set; }
        public string BidaTableStatusName { get; set; }
        public string BidaTableTypeId { get; set; }
        public string BidaTableTypeName { get; set; }
        public DateTime? StartTime { get; set; } // thời gian bắt đầu - nếu bàn này đang chơi
        public string? StartTimeString { get; set; }
        public TimeSpan? PlayTime { get; set; }
        public virtual IEnumerable<BidaTableSession>? BidaTableSession { get; set; }
        public string? OrderParrentId { get; set; }
        public decimal Price { get; set; } // giá bàn
        public string PriceString { get; set; }

    }
}
