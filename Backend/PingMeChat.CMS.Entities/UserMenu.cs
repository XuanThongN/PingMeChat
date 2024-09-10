using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities
{
    public class UserMenu
    {
        [Key, Column(Order = 0)]
        public string AccountId { get; set; }
        [Key, Column(Order = 1)]
        public string MenuId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }
    }
}
