using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Reports.Dto
{
    public class ReportDashboardOverviewDto
    {
        public int TotalOrders { get; set; }
        public int TotalOrderDone { get; set; }
        public int TotalOrderDebt { get; set; }
        public int TotalOrderCancel { get; set; }

        public decimal TotalCash { get; set; } // tổng lượng tiền mặt
        public decimal TotalTransfer { get; set; } // tổng tiền chuyển khoản

        // TH hoàn thành
        public decimal TotalPurchaseCostDone { get; set; } // tổng chi phí mua hàng
        public decimal TotalDiscountDone { get; set; } // tổng giảm giá
        public decimal TotalAmountAfterDiscountDone { get; set; } // Tổng tiền sau giảm giá
        public decimal TotalRevenueDone { get; set; } // Tổng doanh thu
        public decimal ProfitDone { get; set; } // Tổng lợi nhuận

        // TH nợ
        public decimal TotalPurchaseCostDebt { get; set; } // tổng chi phí mua hàng
        public decimal TotalDiscountDebt { get; set; } // tổng giảm giá
        public decimal TotalAmountAfterDiscountDebt { get; set; } // Tổng tiền sau giảm giá
        public decimal TotalRevenueDebt { get; set; } // Tổng doanh thu
        public decimal ProfitDebt { get; set; } // Tổng lợi nhuận
    }
}
