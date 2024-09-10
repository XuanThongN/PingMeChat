using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderPayDto
    {
        
        public string BidaTableId { get; set; }
        public string? Description { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public DiscountType DiscountType { get; set; } = DiscountType.Amount;
        public decimal DiscountNumber { get; set; } = 0; // số tiền giảm giá
        public int DiscountPercent { get; set; } = 0; // % giảm giá
        // public Account CreatedUser { get; set; }
        public string? OrderParentId { get; set; }
        public string? CustomerId { get; set; }
        public Status Status { get; set; } = Status.Done;

    }
}
