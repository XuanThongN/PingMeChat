using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Reports.Dto
{
    public class ReportOverViewDto : ReadDto
    {
        public DateTime StartDate { get; set; }
        public string StartDateString { get; set; }
        public DateTime EndDate { get; set; }
        public string EndDateString { get; set; }
        public int TotalPlays { get; set; } // tổng số lượt chơi
        public decimal MainRevenue { get; set; } // doanh thu chính (giờ chơi)
        public decimal SecondaryRevenue { get; set; } // doanh thu phụ (bán nước, bimbim)
        public decimal TotalRevenue { get; set; } //Tổng doanh thu ( có cả nợ)
        public decimal TotalDebt { get; set; } //Tổng tiền còn nợ của khách hàng

        public List<ReportDetails> ReportDetails { get; set; }
        public List<ReportDetailsProduct> ReportDetailsProducts { get; set; }
        public List<ReportDetailsBida> ReportDetailsBidas { get; set; }
    }
}
