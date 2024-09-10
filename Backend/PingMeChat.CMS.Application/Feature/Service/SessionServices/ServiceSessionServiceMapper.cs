using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.SessionServices.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.SessionServices
{
    public class ServiceSessionServiceMapper : Profile
    {
        public ServiceSessionServiceMapper()
        {
            CreateMap<ServiceSession, SessionServiceCreateDto>().ReverseMap();
            CreateMap<ServiceSession, SessionServiceDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ReverseMap();
            CreateMap<ServiceSession, SessionServiceUpdateDto>().ReverseMap();
        }
    }
}
