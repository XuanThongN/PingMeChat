using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Groups.Dto
{
    public class GroupDto : ReadDto
    {
        public decimal? OrgId { get; set; }

        [MaxLength(50)]
        public string GroupName { get; set; }

        public bool Status { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public bool? IsDataMigration { get; set; }
    }
}
