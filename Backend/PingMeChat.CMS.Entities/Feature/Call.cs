using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Feature
{
    // Call entity
    public class Call : AuditableBaseEntity
    {
        public string ChatId { get; set; }
        public CallStatus CallStatus { get; set; }
        public CallType CallType { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime? EndTime { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual ICollection<CallParticipant> CallParticipants { get; set; }
    }

    public enum CallStatus
    {
        InProgress,
        Completed,
        Missed
    }

    public enum CallType
    {
        Audio,
        Video
    }


}
