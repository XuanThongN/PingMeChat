using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{

    public enum BidaTableStatus
    {
        [Display(Name = "Bàn trống")]
        Available = 0, // bàn rảnh
        [Display(Name = "Đang chơi")]
        Playing = 1, // đang chơi
        [Display(Name = "Tách giờ")] // phục vụ cho tách giờ
        SplitHour = 2, // tách giờ
        [Display(Name = "Chuyển bàn")] // phục vụ cho chuyển bàn
        ChangeTable = 3, // chuyển bàn
        [Display(Name = "Bảo trì")] // bàn đang bảo trì hoặc bị hỏng
        Maintenance = 4, // bảo trì
    }
    public  class BidaTable : AuditableBaseEntity
    {
        public BidaTable()
        {
            BidaTableSessions = new HashSet<BidaTableSession>();
        }
        public string Code { get; set; }
        public BidaTableStatus BidaTableStatus { get; set; }
        public string BidaTableTypeId { get; set; }
        public string? OrderParrentId { get; set; } // id của order cha
        public virtual BidaTableType BidaTableType { get; set; }
        public virtual IEnumerable<BidaTableSession>? BidaTableSessions { get; set; }

    }

   
}
