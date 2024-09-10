using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    public enum InventoryType
    {
        [Display(Name = "Chờ xác nhận")]
        Waiting,
        [Display(Name = "Đã xác nhận")]
        Confirmed,
        [Display(Name = "Đã hủy")]
        Cancelled

    }
    // phiếu nhập kho
    // CreatedDate: Ngày nhập
    // CreatedBy: Người nhập
    public partial class InventoryImport : AuditableBaseEntity
    {
        public InventoryImport()
        {
            InventoryDetailsImports = new List<InventoryDetailsImport>();
        }
        public string Code { get; set; }
        public string PartnerName { get; set; } // Đối tác hoặc tên người giao
        public InventoryType InventoryType { get; set; }
        // cấu hình nó như 1 dạng json trong database
        public virtual List<InventoryDetailsImport>? InventoryDetailsImports { get; set; }
    }

}
