using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.InventoryImports.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.InventoryImports
{
    public class InventoryImportMapper : Profile
    {
        public InventoryImportMapper()
        {

            CreateMap<InventoryImport, InventoryImportCreateDto>().ReverseMap();
            CreateMap<InventoryImport, InventoryImportUpdateDto>().ReverseMap();
            CreateMap<InventoryImport, InventoryImportDto>()
                .ForMember(d => d.InventoryTypeName, o => o.MapFrom(src => src.InventoryType.GetDescription()))
                .ForMember(d => d.TotalAmount, o => o.MapFrom(src => src.InventoryDetailsImports != null ? src.InventoryDetailsImports.Sum(x=> x.Quantity * x.Cost) : 0))

                .ReverseMap();

        }
    }
}
