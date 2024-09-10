using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.SessionServices.Dto
{
    public class SessionServiceDto : ReadDto
    {
        public int Quantity { get; set; } // số lượng
        public decimal Price { get; set; } // giá hiện tại thời điểm sử dụng
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string BidaTableSessionId { get; set; }
    }
}
