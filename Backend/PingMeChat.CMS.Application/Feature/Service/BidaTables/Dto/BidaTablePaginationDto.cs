using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto
{
    public class BidaTablePaginationFilterDto
    {
        public string? Code { get; set; }
        public BidaTableStatus? BidaTableStatus { get; set; }
        public string? BidaTableTypeId { get; set; }
    }
}
