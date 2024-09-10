using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Application.Feature.Service.SessionServices.Dto;
using PingMeChat.CMS.Entities.Feature;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.BidaTableSessionServicess.Dto
{
    public class ServiceSessionPaymentDto
    {
        public string Code { get; set; } // mã bàn 
        public string Name { get; set; } // tên bàn 
        public decimal Price { get; set; } // Giá bàn
        public int TotalTime { get; set; }  // tổng thời gian chơi
        public string TotalTimeString { get; set; }  // tổng thời gian chơi
        public decimal TotalPrice { get; set; } // tổng tền bàn
        public DiscountType DiscountType { get; set; }
        public decimal? Discount { get; set; } // chiết khấu
        public int DiscountPercent { get; set; } // % giảm giá
        public decimal TotalServicePrice { get; set; } // tổng tiền dịch vụ sử dụng
        public List<SessionServiceDto> SessionServiceDtos { get; set; }
    }
}
