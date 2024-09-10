using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.InventoryImports.Dto
{
    public class InventoryImportDto : ReadDto
    {
        public InventoryImportDto()
        {
            InventoryDetailsImports = new List<InventoryDetailsImport>();
        }
        public string Code { get; set; }
        public string PartnerName { get; set; } // Đối tác hoặc tên người giao
        public InventoryType InventoryType { get; set; }
        public string InventoryTypeName { get; set; }
        public decimal TotalAmount { get; set; }
        // cấu hình nó như 1 dạng json trong database
        public virtual List<InventoryDetailsImport>? InventoryDetailsImports { get; set; }
    }
}
