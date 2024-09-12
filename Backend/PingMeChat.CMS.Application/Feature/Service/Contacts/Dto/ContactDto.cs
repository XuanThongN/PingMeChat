using PingMeChat.CMS.Application.Common.Dto;



namespace PingMeChat.CMS.Application.Feature.Service.Contacts.Dto
{
    public class ContactDto : ReadDto
    {
        public string? Name { get; set; }
        public string? NickName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
    }
}
