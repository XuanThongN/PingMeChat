using PingMeChat.CMS.Entities.Feature;
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
    public class RoleMenuDto
    {
        public string RoleId { get; set; }
        public string MenuId { get; set; }
    }
}
