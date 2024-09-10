using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Reports.Dto
{
    public class ReportDetailsProduct
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; } // tương ứng với code trong bảng product
        public int QuanlityBefore { get; set; } // lượng tồn đầu kỳ
        public int QuanlityAfter { get; set; } // lượng tồn cuối kỳ
        public int Quanlity { get; set; } // tổng lượng xuất
        public decimal Expense { get; set; } //Chi phhí dv/sp
        public decimal Revenue { get; set; } // doanh thu
        public decimal GrossProfit { get; set; } // lợi nhuận gộp

    }
}
