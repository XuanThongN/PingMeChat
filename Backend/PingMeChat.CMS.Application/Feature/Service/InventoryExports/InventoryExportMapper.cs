using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.InventoryExports.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.InventoryExports
{
    public class InventoryExportMapper : Profile
    {
        public InventoryExportMapper()
        {

            CreateMap<InventoryExport, InventoryExportCreateDto>().ReverseMap();
            CreateMap<InventoryExport, InventoryExportUpdateDto>().ReverseMap();
            CreateMap<InventoryExport, InventoryExportDto>()
                .ForMember(d => d.InventoryTypeName, o => o.MapFrom(src => src.InventoryType.GetDescription()))
                .ForMember(d => d.TotalAmount, o => o.MapFrom(src => src.InventoryDetailsExports != null ? src.InventoryDetailsExports.Sum(x=> x.Quantity * x.Price) : 0))

                .ReverseMap();

        }
    }
}
