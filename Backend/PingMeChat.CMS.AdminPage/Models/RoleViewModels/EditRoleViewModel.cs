using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PingMeChat.CMS.EntityFrameworkCore;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using PingMeChat.CMS.Entities;

namespace PingMeChat.CMS.AdminPage.Models.RoleViewModels
{
    public class EditRoleViewModel
    {
        public IdentityRole Role { get; set; }
   /*     public IEnumerable<CMSIdentityUser> Members { get; set; }
        public IEnumerable<CMSIdentityUser> NonMembers { get; set; }*/
    }
}
