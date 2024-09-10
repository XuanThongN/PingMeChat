using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.InventoryImports.Dto
{
    public class InventoryImportCreateDto : CreateDto
    {
        public string PartnerName { get; set; } // Đối tác hoặc tên người giao
        public virtual List<InventoryDetailsImport>? InventoryDetailsImports { get; set; }
    }
}
