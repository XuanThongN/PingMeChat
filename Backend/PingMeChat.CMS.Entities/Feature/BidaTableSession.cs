using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    public enum SessionStatus
    {
        [Display(Name = "Kết thúc")]
        Available = 0, // Kết thúc phiên chơi
        [Display(Name = "Đang chơi")]
        Playing = 1, // đang chơi
        //[Display(Name = "Đã kết thúc")]
        //Ended = 2, // đã kết thúc
    }
    // phiên chơi
    public class BidaTableSession : AuditableBaseEntity
    {
        public BidaTableSession()
        {
            ServiceSessions = new HashSet<ServiceSession>();
        }
        public DateTime StartTime { get; set; } // giờ bắt đầu
        public DateTime? EndTime { get; set; } // giờ  kết thúc 
        public SessionStatus SessionStatus { get; set; }
        public string BidaTableId { get; set; }
        public virtual BidaTable BidaTable { get; set; }
        
        public virtual IEnumerable<ServiceSession> ServiceSessions { get; set; }
    }
}
