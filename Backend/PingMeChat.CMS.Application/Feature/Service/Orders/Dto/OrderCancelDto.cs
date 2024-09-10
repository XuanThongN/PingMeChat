using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderCancelDto
    {
        public string OrderId { get; set; }
        //public string? CancelReasonId { get; set; }
        public string? CancelContent { get; set; }
        public string? StaffName { get; set; }
    }

}
