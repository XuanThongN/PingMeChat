using Microsoft.AspNetCore.Http;

namespace PingMeChat.CMS.Application.Feature.Service.Users.Dto
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UpdateAvatarDto {
        public IFormFile avatar { get; set; }
    }
}
