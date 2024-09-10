using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderSearchDto : RequestDataTable
    {
        public string? Code { get; set; }
        public Status? OrderStatus { get; set; }
        public DiscountType? DiscountType { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? StaffName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

    }
}
