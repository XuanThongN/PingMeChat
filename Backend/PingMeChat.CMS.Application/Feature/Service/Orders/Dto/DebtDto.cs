using PingMeChat.CMS.Application.Feature.Service.Customers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class DebtDto
    {
        public string? CustomerId { get; set; } // TH đã có khách hàng trong hệ thống
        public CustomerCreateDto? Customer { get; set; } // TH khách hàng chưa có trong hệ thống
        public OrderPayDto OrderPay { get; set; }
    }
}
