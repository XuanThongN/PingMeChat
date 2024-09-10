using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderDetailsUpdateDto
    {
        public string Id { get; set; }
        public string? OrderParrentId { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
