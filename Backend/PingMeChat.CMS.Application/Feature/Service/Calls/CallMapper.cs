using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Calls.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Users;

namespace PingMeChat.CMS.Application.Feature.Service.Calls
{
    public class CallMapper : Profile
    {
        public CallMapper()
        {
            CreateMap<Call, CallDto>().ReverseMap();
            CreateMap<Call, CallCreateDto>().ReverseMap();
            CreateMap<Call, CallUpdateDto>().ReverseMap();
        }
    }
}
