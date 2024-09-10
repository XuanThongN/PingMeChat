using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Groups.Dto
{
    public class GroupSearchDto : RequestDataTable
    {
        public bool? Status { get; set; }
    }
}
