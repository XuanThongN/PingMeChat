using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Products.Dto
{
    public class ProductCreateDto : CreateDto
    {
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; } // giá bán
     //   public decimal Cost { get; set; } // giá nhập
    //    public int Quantity { get; set; } // số lưọng
    }
}
