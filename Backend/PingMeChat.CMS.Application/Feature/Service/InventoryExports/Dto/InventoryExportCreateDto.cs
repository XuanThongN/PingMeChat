using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.InventoryExports.Dto
{
    public class InventoryExportCreateDto : CreateDto
    {
        public string PartnerName { get; set; } // Đối tác hoặc tên người giao
        public virtual List<InventoryDetailsExport>? InventoryDetailsExports { get; set; }
    }
}
