using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using System;


namespace PingMeChat.CMS.Application.Feature.Service.UserChats.Dto
{
    public class UserChatDto : ReadDto
    {
        public string UserId { get; set; }
        public string ChatId { get; set; }
        public bool IsAdmin { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public AccountDto? UserDto { get; set; }
    }
}
