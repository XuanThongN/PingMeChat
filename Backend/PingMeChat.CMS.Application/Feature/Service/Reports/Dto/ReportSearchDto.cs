using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Reports.Dto
{
    public class ReportSearchDto : RequestDataTable
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Status? Status { get; set; } = Entities.Feature.Status.Done;
    }
}
