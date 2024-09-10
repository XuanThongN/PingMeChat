using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PingMeChat.Shared.Utils.Message.Error;

namespace PingMeChat.CMS.Entities
{
    public class GroupUser
    {
        public string GroupId { get; set; }
        public string AccountId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
    }
}
