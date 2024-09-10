using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities
{
    public class GroupRole
    {
        public string GroupId { get; set; }
        public string RoleId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

    }
}
