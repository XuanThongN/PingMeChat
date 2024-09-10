using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Inventories.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;

namespace PingMeChat.CMS.Application.Feature.Service.Inventorys
{
    public class InventoryMapper : Profile
    {
        public InventoryMapper()
        {
            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Product.Cost))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.CostFormat, opt => opt.MapFrom(src => src.Product.Cost.Value.FormatToCurrency()))
                .ForMember(dest => dest.PriceFormat, opt => opt.MapFrom(src => src.Product.Price.FormatToCurrency()))
                .ForMember(dest => dest.UpdatedDateString, opt => opt.MapFrom(src => src.UpdatedDate != null ? src.UpdatedDate.Value.ToVietNamFormat() : null))

                .ReverseMap();
            
            CreateMap<Inventory, InventoryCreateDto>().ReverseMap();
            CreateMap< Inventory, InventoryUpdateDto>().ReverseMap();
        }
    }
}
