using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Roles.Dto
{
    public class RoleUpdateDto : UpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

    }
}
