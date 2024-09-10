using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // dịch vụ - sản phẩm sử dụng trong phiên chơi
    public class ServiceSession : AuditableBaseEntity
    {
        public int Quantity { get; set; } // số lượng
        [Column("decimal(18,2)")]
        public decimal Price { get; set; } // giá hiện tại thời điểm sử dụng
        public string ProductId { get; set; }
        public string BidaTableSessionId { get; set; }
        public virtual Product Product { get; set; }
        public virtual BidaTableSession BidaTableSession { get; set; }
    }
}
