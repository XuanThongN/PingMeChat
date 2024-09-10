using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Users.Dto
{
    public class UserCreateDto : CreateDto
    {
        [Required]
        [Email]
        public string Email { get; set; }
        
        [Required]
        [Username]
        public string UserName { get; set; } // UserName

        [Required]
        [Password]
        public string Password { get; set; }

        [Required]
        [ConfirmPassword("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }
        public List<string>? GroupIds { get; set; }

    }
}
