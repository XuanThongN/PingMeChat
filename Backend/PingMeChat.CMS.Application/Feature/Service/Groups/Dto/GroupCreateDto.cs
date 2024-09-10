using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Groups.Dto
{
    public class GroupCreateDto : CreateDto
    {
        public decimal? OrgId { get; set; }

        [MaxLength(50)]
        public string GroupName { get; set; }

        public bool Status { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
        
        public bool? IsDataMigration { get; set; }

        public List<string>? UserIds { get; set; }

        public List<string>? RoleIds { get; set; }

    }
}
