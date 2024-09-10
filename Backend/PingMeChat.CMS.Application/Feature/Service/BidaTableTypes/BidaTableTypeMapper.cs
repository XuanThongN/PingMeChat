using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Enum;
using PingMeChat.Shared;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables
{
    public class BidaTableTypeMapper : Profile
    {
        public BidaTableTypeMapper()
        {
            CreateMap<BidaTableType, BidaTableTypeDto>()
                .ForMember(d => d.CreatedDateString, o => o.MapFrom(src => src.CreatedDate.Value.ToVietNamFormatDate()))
                .ForMember(d => d.UpdatedDateString, o => o.MapFrom(src => src.UpdatedDate.Value.ToVietNamFormatDate()))
                .ReverseMap();
            CreateMap<BidaTableType, BidaTableTypeCreateDto>().ReverseMap();
            CreateMap<BidaTableType, BidaTableTypeUpdateDto>().ReverseMap();
        }
    }
}
