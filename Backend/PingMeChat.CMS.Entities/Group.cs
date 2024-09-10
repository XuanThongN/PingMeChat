using PingMeChat.CMS.Entities.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PingMeChat.CMS.Entities
{
    public class Group : AuditableBaseEntity
    {
        public Group()
        {
            GroupRoles = new HashSet<GroupRole>();
            GroupUsers = new HashSet<GroupUser>();
        }
        [Column(TypeName = "decimal(4,0)")]
        public decimal? OrgId { get; set; }

        [MaxLength(50)]
        public string GroupName { get; set; }

        public bool Status { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public bool? IsDataMigration { get; set; }

        public virtual ICollection<GroupRole> GroupRoles { get; set; }
        public virtual ICollection<GroupUser> GroupUsers { get; set; }
    }
}
