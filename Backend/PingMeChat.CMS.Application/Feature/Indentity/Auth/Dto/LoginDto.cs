using PingMeChat.CMS.Application.Common.Attributes;
using System.ComponentModel.DataAnnotations;


namespace PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto
{
    public class LoginDto
    {
        [Required]
        [Username(6, 225)] // UsernameAttribute
        public string UserName { get; set; }

        [Required]
        [Password(8, 225)] // PasswordAttribute
        public string Password { get; set; }
    }
}
