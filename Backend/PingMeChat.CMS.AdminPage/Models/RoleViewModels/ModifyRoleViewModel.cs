using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PingMeChat.CMS.AdminPage.Models.RoleViewModels
{
    public class ModifyRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }
        public string[]? IdsToAdd { get; set; }
        public string[]? IdsToRemove { get; set; }

    }
}
