using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using PingMeChat.Shared.Enum;

namespace PingMeChat.CMS.Application.Feature.Service.Orders
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, OrderCreateDto>()
                .ForMember(d => d.OrderDetails, o => o.MapFrom(src => src.OrderDetails))
                .ReverseMap();
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.OrderDetails, o => o.MapFrom(src => src.OrderDetails))
                .ForMember(d => d.StatusName, o => o.MapFrom(src => src.Status.GetDescription()))
                .ForMember(d => d.TotalServiceAmount, o => o.MapFrom(src => src.OrderDetails.Any() ? src.OrderDetails.Sum(x => x.SubTotal) : 0))
                .ForMember(d => d.DiscountTypeName, o => o.MapFrom(src => src.DiscountType.Value.GetDescription()))
                .ForMember(d => d.PaymentMethodName, o => o.MapFrom(src => src.PaymentMethod.GetDescription()))
                .ForMember(d => d.StartTimeString, o => o.MapFrom(src => src.StartTime.ToVietNamFormat()))
                .ForMember(d => d.EndTimeString, o => o.MapFrom(src => src.EndTime.ToVietNamFormat()))
                .ForMember(d => d.OrderDateString, o => o.MapFrom(src => src.OrderDate.ToVietNamFormat()))
                .ForMember(d => d.CreatedDateString, o => o.MapFrom(src => src.CreatedDate.Value.ToVietNamFormat()));
            CreateMap<OrderDetail, OrderDetailDto>()
                 .ReverseMap();


            CreateMap<Order, OrderHistory>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore()) // Bỏ qua mapping cho trường Id của OrderHistory
                 .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id)) // Mapping giá trị của trường OrderId từ trường Id của Order
                .ReverseMap();

        }
    }
}
