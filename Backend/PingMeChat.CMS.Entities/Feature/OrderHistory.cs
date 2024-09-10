using PingMeChat.CMS.Entities.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // lịch sử hủy đơn hàng
    public  class OrderHistory : AuditableBaseEntity
    {
        public string Code { get; set; }
        public string BidaTableCode { get; set; }
        public DateTime OrderDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal BidaTableAmount { get; set; }  // tiền bàn
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }  // tổng tiền
        public decimal TotalAmountAllOrder { get; set; }  // tổng tiền của tất cả các order (gồm các order con)
        public DiscountType? DiscountType { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; } // chiết khấu
        public int? DiscountPercent { get; set; } // % giảm giá

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Tax { get; set; } // thuế
        public PaymentMethod PaymentMethod { get; set; }
        public Status Status { get; set; }
        public string? Description { get; set; }
        public string? StaffName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<OrderDetail>? OrderDetails { get; set; } // được lưu vào dưới dạng json

        // bill tạm phục vụ chức năng tách bàn và tách giờ
        public string? ParrentId { get; set; }
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string? OrderId { get; set; } // bảng chính
        public string? CancelContent { get; set; }

    }


}
