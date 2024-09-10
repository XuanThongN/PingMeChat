using PingMeChat.CMS.Entities.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PingMeChat.CMS.Entities
{
    public class Menu : AuditableBaseEntity
    {
        public Menu()
        {
            RoleMenus = new HashSet<RoleMenu>();
            UserMenus = new HashSet<UserMenu>();
            Children = new HashSet<Menu>();
        }

        public string Title { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        [Display(Name = "Mã menu cha")]
        public string? ParentId { get; set; }
        public bool MenuType { get; set; }
        public int SortOrder { get; set; }
        public List<MvcActionInfo>? Access { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("ParentId")]
        public virtual ICollection<Menu> Children { get; set; }
        public virtual ICollection<UserMenu> UserMenus { get; set; }
        public virtual ICollection<RoleMenu> RoleMenus { get; set; }
    }
}
