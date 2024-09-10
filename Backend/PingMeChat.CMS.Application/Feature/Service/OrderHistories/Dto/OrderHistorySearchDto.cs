using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys.Dto
{
    public class OrderHistorySearchDto : RequestDataTable
    {
      public string OrderId { get; set; }

    }
}
