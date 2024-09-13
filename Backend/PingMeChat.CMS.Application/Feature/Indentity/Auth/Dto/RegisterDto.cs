using PingMeChat.CMS.Application.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto
{
    public class RegisterDto
    {
        [Required]
        [Email]
        public string Email { get; set; }

        [Required]
        [Username(6, 225)]
        public string UserName { get; set; }

        [Required]
        [Password(8, 225)]
        public string Password { get; set; }

        [Required]
        [ConfirmPassword("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(225, MinimumLength = 3, ErrorMessage = "Tên phải có ít nhất 3 ký tự và nhiều nhất là 225 ký tự")]
        public string FullName { get; set; }

        [Required]
        [VietnamPhoneNumberAttribute]
        public string Phone { get; set; }
    }
}
