using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PingMeChat.CMS.Entities
{
    public class RoleMenu
    {
        [Key, Column(Order = 0)]
        public string RoleId { get; set; }
        [Key, Column(Order = 1)]
        public string MenuId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }
    }
}
