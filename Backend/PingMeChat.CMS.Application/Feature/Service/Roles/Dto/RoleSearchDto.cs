using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Roles.Dto
{
    public class RoleSearchDto : RequestDataTable
    {
        public bool? Status { get; set; }
    }
}
