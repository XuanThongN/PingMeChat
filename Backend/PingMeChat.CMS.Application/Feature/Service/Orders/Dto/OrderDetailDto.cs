using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderDetailDto
    {
        public int Quantity { get; set; } // số lượng

        public decimal UnitPrice { get; set; } // đơn giá

        public decimal SubTotal { get; set; } // tổng tiền
        public string ProductName { get; set; }
        public string? ProductId { get; set; }

    }
}
