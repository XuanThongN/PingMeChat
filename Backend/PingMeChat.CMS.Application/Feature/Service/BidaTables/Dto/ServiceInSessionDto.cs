using PingMeChat.CMS.Application.Feature.Service.SessionServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto
{
    public class ServiceInSessionDto
    {
        public string BidaTableId { get; set; }
        public List<SessionServiceCreateDto> SessionServiceCreateDtos { get; set; }
    }
}
