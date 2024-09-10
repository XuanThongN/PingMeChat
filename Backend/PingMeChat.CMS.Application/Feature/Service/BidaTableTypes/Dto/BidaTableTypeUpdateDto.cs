using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto
{
    public class BidaTableTypeUpdateDto : UpdateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
