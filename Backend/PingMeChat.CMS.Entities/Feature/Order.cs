using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    public enum PaymentMethod
    {

        [Display(Name = "Tiền mặt")]
        Cash,
        [Display(Name = "Thanh toán qua thẻ")]
        CreditCard,
        [Display(Name = "Chuyển tiền")]
        BankTransfer
    }

    public enum Status
    {
        [Display(Name = "Hoàn thành")]
        Done,
        [Display(Name = "Đã hủy")]
        Cancelled,
        [Display(Name = "Đang nợ")]
        Debt ,
        [Display(Name = "Chờ xác nhận")]
        Pending 
    }
    public enum DiscountType
    {
        [Display(Name = "%")]
        Percent, // phần trăm
        [Display(Name = "₫")]
        Amount // số tiền 
    }
    // đơn hàng
    public  class Order : AuditableBaseEntity
    {
        //public Order() {
        //    OrderDetails = new HashSet<OrderDetail>();
        //}
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

        // bill tạm phục vụ chức năng chuyển bàn và tách giờ
        public string? ParrentId { get; set; }
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string? CancelContent { get; set; }
    }
}
