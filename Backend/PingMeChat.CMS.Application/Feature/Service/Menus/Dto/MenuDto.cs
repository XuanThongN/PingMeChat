using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Menus.Dto
{
    public class MenuDto : ReadDto
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string? ParentId { get; set; }
        public bool MenuType { get; set; }
        public int SortOrder { get; set; }
        public List<MvcActionInfo> Access { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<MenuDto> Children { get; set; }
        public virtual ICollection<UserMenuDto> UserMenus { get; set; }
        public virtual ICollection<RoleMenuDto> RoleMenus { get; set; }
    }
}
