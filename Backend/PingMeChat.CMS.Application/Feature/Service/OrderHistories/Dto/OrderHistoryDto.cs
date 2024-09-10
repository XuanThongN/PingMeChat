using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys.Dto
{
    public class OrderHistoryDto : ReadDto
    {
        public string Code { get; set; }
        public string BidaTableCode { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal BidaTableAmount { get; set; }  // tiền bàn
        public decimal TotalAmount { get; set; }  // tổng tiền
        public decimal TotalAmountAllOrder { get; set; }  // tổng tiền của tất cả các order (gồm các order con)
        public decimal? TotalServiceAmount { get; set; } // tổng tiền dịch vụ

        public DiscountType? DiscountType { get; set; }
        public string? DiscountTypeName { get; set; } // tên loại giảm giá

        public decimal? Discount { get; set; } // chiết khấu
        public int? DiscountPercent { get; set; } // % giảm giá

        public decimal? Tax { get; set; } // thuế
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentMethodName { get; set; } // phương thức thanh toán

        public Status Status { get; set; }
        public string? StatusName { get; set; } // tên trạng thái
        
        public string? Description { get; set; }
        public string? StaffName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<OrderDetail>? OrderDetails { get; set; } // được lưu vào dưới dạng json

        // bill tạm phục vụ chức năng chuyển bàn và tách giờ
        public string? ParrentId { get; set; }
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string? OrderId { get; set; } // bảng chính
        public string? CancelContent { get; set; }

    }

}
