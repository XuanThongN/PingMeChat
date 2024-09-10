using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.InventoryExports.Dto
{
    public class InventoryExportSearchDto : RequestDataTable
    {
        public InventoryType? InventoryType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}
