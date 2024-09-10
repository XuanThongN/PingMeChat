using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Products.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingMeChat.Shared;

namespace PingMeChat.CMS.Application.Feature.Service.Products
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.PriceString, o => o.MapFrom(s => s.Price.FormatToCurrency()))
                .ForMember(d => d.CostString, o => o.MapFrom(s => s.Cost.HasValue ? s.Cost.Value.FormatToCurrency() : ""))
                .ReverseMap();
            CreateMap<Product, ProductCreateDto>().ReverseMap();
            // Nếu property có giá trị null thì sẽ giữ nguyên giá trị cũ của property đó
            CreateMap<ProductUpdateDto, Product>().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
