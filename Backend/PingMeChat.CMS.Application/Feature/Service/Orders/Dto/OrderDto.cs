using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Service.Customers.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderDto : ReadDto
    {
        public string Code { get; set; }
        public string BidaTableCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderDateString { get; set; }
        public decimal BidaTableAmount { get; set; } // tiền bàn
        public decimal TotalAmount { get; set; }
        public decimal? TotalServiceAmount { get; set; } // tổng tiền dịch vụ
        public decimal? TotalAmountAllOrder { get; set; }  // tổng tiền của tất cả các order (gồm các order con)
        public decimal Discount { get; set; } // chiết khấu
        public int? DiscountPercent { get; set; } // % chiết khấu

        public decimal Tax { get; set; } // thuế
        public PaymentMethod PaymentMethod { get; set; } // phương thức thanh toán
        public string? PaymentMethodName { get; set; } // phương thức thanh toán
        public Status Status { get; set; } // trạng thái
        public string? StatusName { get; set; } // tên trạng thái
        public DiscountType DiscountType { get; set; } // loại giảm giá
        public string? DiscountTypeName { get; set; } // tên loại giảm giá
        public string Description { get; set; } // mô tả
        public string? StaffName { get; set; } // tên nhân viên
        public DateTime StartTime { get; set; } // giờ bắt đầu
        public string StartTimeString { get; set; } // giờ bắt đầu
        public DateTime EndTime { get; set; } // giờ  kết thúc
        public string EndTimeString { get; set; } // giờ  kết thúc
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        // bill tạm phục vụ chức năng chuyển bàn và tách giờ
        public string? ParrentId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
    }

}
