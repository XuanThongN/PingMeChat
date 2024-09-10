using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // chi tiết phiếu xuất kho
    public class InventoryDetailsExport : BaseEntity
    {
        public int Quantity { get; set; } // số lưọng
        public decimal Cost { get; set; } // giá nhập
        public decimal Price { get; set; } // giá bán
        public string ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? InventoryExportId { get; set; }
    }
}
