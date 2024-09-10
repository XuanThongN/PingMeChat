using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto
{
    public class BidaTableSearchDto : RequestDataTable
    {
        public string? Code { get; set; }
        public BidaTableStatus? BidaTableStatus { get; set; }
        public string? BidaTableTypeId { get; set; }
    }
}
