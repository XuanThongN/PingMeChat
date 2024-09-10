using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderUpdateDto : UpdateDto
    {
        public string Code { get; set; }
        public string BidaTableCode { get; set; } // mã bàn
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; } // thuế
        public PaymentMethod PaymentMethod { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public string StaffId { get; set; }
        //public string? CustomerId { get; set; }
        public string? CustomerId { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
