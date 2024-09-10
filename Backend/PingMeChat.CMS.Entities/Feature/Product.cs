using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // sản phẩm - dịch vụ
    public partial class Product : AuditableBaseEntity
    {
       
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]

        public decimal Price { get; set; } // giá bán
        [Column(TypeName = "decimal(18,2)")]

        public decimal? Cost { get; set; } // giá nhập
        public int Quantity { get; set; } // số lưọng

    }

    public partial class Product : AuditableBaseEntity
    {
        public virtual IEnumerable<InventoryExport> InventoryExports { get; set; }
        public virtual IEnumerable<InventoryImport> InventoryImports { get; set; }


    }
}
