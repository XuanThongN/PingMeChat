using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Reports.Dto
{
    public class ReportDetails
    {
        public DateTime Date { get; set; }
        public decimal MainRevenue { get; set; } // doanh thu chính (giờ chơi)
        public decimal SecondaryRevenue { get; set; } // doanh thu phụ (bán nước, bimbim)
        public decimal Expense { get; set; } //Chi phhí dv/sp
        public decimal Discount { get; set; } // Giảm giá
        public decimal GrossProfit { get; set; } // Lãi gộp
        public string StatusName { get; set; }
        public Status Status { get; set; }
    }
}
