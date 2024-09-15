using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Groups.Dto
{
    public class GroupPaginationFilterDto
    {
        public string? GroupName { get; set; }

        public bool? Status { get; set; }
    }
}
