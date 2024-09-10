using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Enum;
using PingMeChat.Shared;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTables
{
    public class BidaTableMapper : Profile
    {
        public BidaTableMapper()
        {
            CreateMap<BidaTable, BidaTableDto>()
                .ForMember(d => d.BidaTableTypeName, o => o.MapFrom(src => src.BidaTableType.Name))
                .ForMember(d => d.BidaTableStatusName, o => o.MapFrom(src => src.BidaTableStatus.GetDescription()))
                .ForMember(d => d.CreatedDateString, o => o.MapFrom(src => src.CreatedDate.Value.ToVietNamFormatDate()))
                .ForMember(d => d.UpdatedDateString, o => o.MapFrom(src => src.UpdatedDate.Value.ToVietNamFormatDate()))
                .ForMember(d => d.Price, o => o.MapFrom(src => src.BidaTableType.Price))
                .ForMember(d => d.PriceString, o => o.MapFrom(src => src.BidaTableType.Price.FormatToCurrency()))
                .ForMember(d => d.StartTime, o => o.MapFrom(src => src.BidaTableSessions == null ? DateTime.Now : src.BidaTableSessions.Where(x => x.SessionStatus == SessionStatus.Playing).OrderByDescending(x =>x.CreatedDate).FirstOrDefault() == null ? DateTime.Now : src.BidaTableSessions.Where(x => x.SessionStatus == SessionStatus.Playing).OrderByDescending(x => x.CreatedDate).FirstOrDefault().StartTime))
                .ForMember(d => d.StartTimeString, o => o.MapFrom(src => src.BidaTableSessions == null ? "" : src.BidaTableSessions.Where(x => x.SessionStatus == SessionStatus.Playing).OrderByDescending(x => x.CreatedDate).FirstOrDefault() == null ? "" : src.BidaTableSessions.Where(x => x.SessionStatus == SessionStatus.Playing).OrderByDescending(x => x.CreatedDate).FirstOrDefault().StartTime.ToVietNamFormatTime()))
                .ForMember(d => d.PlayTime, o => o.MapFrom(src => src.BidaTableSessions == null ? (TimeSpan?)null : src.BidaTableSessions.Where(x => x.SessionStatus == SessionStatus.Playing).OrderByDescending(x => x.CreatedDate).FirstOrDefault() == null ? (TimeSpan?)null : DateTime.Now -  src.BidaTableSessions.Where(x => x.SessionStatus == SessionStatus.Playing).OrderByDescending(x => x.CreatedDate).FirstOrDefault().StartTime))
                .ReverseMap();
            CreateMap<BidaTable, BidaTableCreateDto>().ReverseMap();
            CreateMap<BidaTable, BidaTableUpdateDto>().ReverseMap();
            CreateMap<BidaTableDto, BidaTableUpdateDto>().ReverseMap();
        }
    }
}