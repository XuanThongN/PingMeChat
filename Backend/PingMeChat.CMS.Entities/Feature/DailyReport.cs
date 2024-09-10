using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // bảng báo cáo ngày
    public class DailyReport :AuditableBaseEntity
    {
        public DateTime ReportDate { get; set; }
        [Column("decimal(18,2)")]
        public decimal TotalRevenue { get; set; }
        [Column("decimal(18,2)")]

        public decimal TotalRefund { get; set; }
        [Column("decimal(18,2)")]

        public decimal TotalDiscount { get; set; }
        [Column("decimal(18,2)")]

        public decimal NetRevenue { get; set; }
        [Column("decimal(18,2)")]

        public decimal TotalProfit { get; set; }
        [Column("decimal(18,2)")]

        public decimal ProfitRate { get; set; }
        public int TotalCustomers { get; set; }
    }
}
