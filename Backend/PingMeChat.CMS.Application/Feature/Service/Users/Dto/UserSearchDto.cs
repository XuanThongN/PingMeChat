using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Users.Dto
{
    public  class UserSearchDto : RequestDataTable
    {
        public bool? IsLocked { get; set; }

    }
}
