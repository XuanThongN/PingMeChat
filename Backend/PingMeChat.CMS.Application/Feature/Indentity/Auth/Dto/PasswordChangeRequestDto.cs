using PingMeChat.CMS.Application.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto
{
    public class PasswordChangeRequestDto
    {
        [Required]
        [Email]
        public string Email { get; set; }
        [Required]
        [Password]
        public string Password { get; set; }
    }
}
