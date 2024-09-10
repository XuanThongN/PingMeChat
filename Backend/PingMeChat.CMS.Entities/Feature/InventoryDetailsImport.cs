using PingMeChat.CMS.Entities.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    public class InventoryDetailsImport : BaseEntity
    {
        public int Quantity { get; set; } // số lưọng
        public decimal Cost { get; set; } // giá nhập
        public string ProductId { get; set; }
        public string? InventoryImport { get; set; }
        public string? ProductName { get; set; }
    }
}
