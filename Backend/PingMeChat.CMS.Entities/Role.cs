using PingMeChat.CMS.Entities.Interfaces;

namespace PingMeChat.CMS.Entities
{
    public class Role : AuditableBaseEntity
    {
        public Role()
        {
            RoleMenus = new HashSet<RoleMenu>();
            GroupRoles = new HashSet<GroupRole>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<RoleMenu> RoleMenus { get; set; }
        public virtual ICollection<GroupRole> GroupRoles { get; set; }
    }
}
