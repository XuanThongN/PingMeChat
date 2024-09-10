using PingMeChat.CMS.Application.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto
{
    public class UpdateAccountInfoDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        //[Required]
        //[Password(8, 225)]
        //public string OldPassword { get; set; }

        [Required]
        [Password(8, 225)]
        public string? NewPassword { get; set; }

    }
}
