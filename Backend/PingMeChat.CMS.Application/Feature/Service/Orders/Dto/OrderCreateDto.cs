using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderCreateDto : CreateDto
    {
        public string Code { get; set; }
        public string BidaTableCode { get; set; } // mã bàn
        public DateTime OrderDate { get; set; }
        public decimal BidaTableAmount { get; set; } // tiền bàn
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountAllOrder { get; set; } // tổng tiền của tất cả các order (gồm các order con)
        public decimal Discount { get; set; }
        public DiscountType DiscountType { get; set; } // loại giảm giá
        public decimal Tax { get; set; } // thuế
        public PaymentMethod PaymentMethod { get; set; } // phương thức thanh toán
        public Status Status { get; set; } // trạng thái
        public string? Description { get; set; } = string.Empty; // mô tả
        public string StaffName { get; set; } // nhân viên
        public DateTime StartTime { get; set; } // giờ bắt đầu
        public DateTime EndTime { get; set; } // giờ  kết thúc
        public decimal DiscountPercent { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        // bill tạm phục vụ chức năng tách bàn và tách giờ
        public string? ParrentId { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
