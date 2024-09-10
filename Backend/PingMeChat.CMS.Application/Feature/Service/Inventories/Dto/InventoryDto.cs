using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Inventories.Dto
{
    public class InventoryDto : ReadDto
    {
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; } // số lượng tồn kho
        public decimal Cost { get; set; } // giá nhập gần nhất
        public decimal Price { get; set; } // giá bán hiện tại
        public string CostFormat { get; set; }
        public string PriceFormat { get; set; }
    }
}
