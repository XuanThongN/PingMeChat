using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.Contacts.Dto
{
    public class ContactUpdateDto : UpdateDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Friend ID is required")]
        public string FriendId { get; set; }

        public string? NickName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Address { get; set; }

        public string? Note { get; set; }
    }
}
