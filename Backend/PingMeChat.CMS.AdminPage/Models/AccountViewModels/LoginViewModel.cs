using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PingMeChat.CMS.AdminPage.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Nhớ tôi?")]
        public bool RememberMe { get; set; }
    }
}
