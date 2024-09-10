using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Reports.Dto
{
    public class ReportDetailsBida
    {
        public string BidaTableId { get; set; }
        public string BidaTableCode { get; set; }
        public string BidaTableTypeName { get; set; }
        public decimal Price { get; set; }
        public double TotalTime { get; set; } // tổng thời gian chơi
        public double ProductivityUsing { get; set; } // Công suất sử dụng
    }
}
