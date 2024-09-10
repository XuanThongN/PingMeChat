using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Roles.Dto;
using PingMeChat.CMS.Entities;
using PingMeChat.Shared;

namespace PingMeChat.CMS.Application.Feature.Service.Roles
{
    public class RoleMapper : Profile
    {
        public RoleMapper()
        {
            CreateMap<Role, RoleDto>()
                 .ForMember(d => d.CreatedDateString, o => o.MapFrom(src => src.CreatedDate.Value.ToVietNamFormat()))
                 .ForMember(d => d.UpdatedDateString, o => o.MapFrom(src => src.UpdatedDate.Value.ToVietNamFormat()))
                .ReverseMap();
            CreateMap<Role, RoleCreateDto>().ReverseMap();
            CreateMap<Role, RoleUpdateDto>().ReverseMap();
        }
    }
}
