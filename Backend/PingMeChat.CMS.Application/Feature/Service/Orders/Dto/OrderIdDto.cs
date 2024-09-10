using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Orders.Dto
{
    public class OrderIdDto
    {
        public string Id { get; set; }
        public Status Status { get; set; } = PingMeChat.CMS.Entities.Feature.Status.Done;
    }
}
