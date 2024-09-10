using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTableSessionServicess
{
    public class BidaTableSessionMapper :Profile
    {
        public BidaTableSessionMapper()
        {
            CreateMap<BidaTableSession, BidaTableSessionCreateDto>().ReverseMap();
            CreateMap<BidaTableSession, BidaTableSessionDto>().ReverseMap();
            CreateMap<BidaTableSession, BidaTableSessionUpdateDto>().ReverseMap();
        }
    }
}
