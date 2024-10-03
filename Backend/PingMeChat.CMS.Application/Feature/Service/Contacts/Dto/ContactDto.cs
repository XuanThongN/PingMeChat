using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.Shared.Enum;



namespace PingMeChat.CMS.Application.Feature.Service.Contacts.Dto
{
    public class ContactDto : ReadDto
    {
        public string? FullName { get; set; }
        public string? NickName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public string? UserId { get; set; }
        public string? ContactUserId { get; set; }
        public ContactStatus? Status { get; set; }
        public AccountDto? User { get; set; }
        public AccountDto? ContactUser { get; set; }
    }
}
