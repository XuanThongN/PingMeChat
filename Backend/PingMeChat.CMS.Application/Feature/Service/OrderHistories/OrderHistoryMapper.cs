using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using PingMeChat.Shared.Enum;

namespace PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys
{
    public class OrderHistoryMapper : Profile
    {
        public OrderHistoryMapper()
        {
            CreateMap<OrderHistory, OrderHistoryCreateDto>()
                .ReverseMap();
            CreateMap<OrderHistory, OrderHistoryDto>()
                 .ForMember(d => d.OrderDetails, o => o.MapFrom(src => src.OrderDetails))
                .ForMember(d => d.StatusName, o => o.MapFrom(src => src.Status.GetDescription()))
                .ForMember(d => d.TotalServiceAmount, o => o.MapFrom(src => src.OrderDetails.Any() ? src.OrderDetails.Sum(x => x.SubTotal) : 0))
                .ForMember(d => d.DiscountTypeName, o => o.MapFrom(src => src.DiscountType.Value.GetDescription()))
                .ForMember(d => d.PaymentMethodName, o => o.MapFrom(src => src.PaymentMethod.GetDescription()))
                .ReverseMap();

        }
    }
}
