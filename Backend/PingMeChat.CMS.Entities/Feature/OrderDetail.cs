using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    public class OrderDetail
    {

        public int Quantity { get; set; } // số lượng
        [Column(TypeName = "decimal(18,2)")]

        public decimal UnitPrice { get; set; } // đơn giá
        [Column(TypeName = "decimal(18,2)")]

        public decimal SubTotal { get; set; } // tổng tiền
        public decimal Cost { get; set; } // Giá nhập

        public string ProductName { get; set; }
        public string ProductId { get; set; }

    }
    public class OrderDetailCreateDto
    {

        public int Quantity { get; set; } // số lượng
        [Column(TypeName = "decimal(18,2)")]

        public string ProductId { get; set; }

    }

    //public partial class OrderDetail : AuditableBaseEntity
    //{
    //    public string OrderId { get; set; }
    //    public virtual Order Order { get; set; }
    //    public string ProductId { get; set; }
    //    public virtual Product Product { get; set; }


    //}
}
