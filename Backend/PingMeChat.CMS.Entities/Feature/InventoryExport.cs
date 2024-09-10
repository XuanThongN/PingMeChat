using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // bảng phiếu xuất kho
    public partial class InventoryExport : AuditableBaseEntity
    {
        public InventoryExport()
        {
            InventoryDetailsExports = new List<InventoryDetailsExport>();
        }
        public string Code { get; set; }
        public string PartnerName { get; set; } // Đối tác hoặc tên người nhận ( khách hàng)
        public InventoryType InventoryType { get; set; }
        // cấu hình nó như 1 dạng json trong database
        public virtual List<InventoryDetailsExport>? InventoryDetailsExports { get; set; }

    }

}
