using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // bảng kho hàng
    public class Inventory : AuditableBaseEntity
    {
        public int Quantity { get; set; } // số lưọng
        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
