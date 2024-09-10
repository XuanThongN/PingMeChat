using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.Users.Dto
{
    public class UserUpdateDto : UpdateDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [Email]
        public string Email { get; set; }
        public bool IsLocked { get; set; }

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }
        public List<string>? GroupIds { get; set; }
    }
}
