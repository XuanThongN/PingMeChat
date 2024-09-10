using PingMeChat.CMS.Application.Common.Dto;

using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys.Dto
{
    public class OrderHistoryCreateDto : CreateDto
    {
        public string? StaffName { get; set; }
        public DateTime CancelDate { get; set; }
        public string? OrderId { get; set; }
        public OrderDto? Order { get; set; }
        public string? CancelReasonId { get; set; }

    }
}
