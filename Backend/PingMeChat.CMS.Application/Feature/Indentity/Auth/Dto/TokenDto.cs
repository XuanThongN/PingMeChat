using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto
{
    public class TokenDto
    {
        public string AccessToken { get; set; } = null!;

      //  [JsonIgnore] // không trả refresh token về phía người dùng hiện ở json, chỉ trả về gán ở cookie trình duyệt

        public string RefreshToken { get; set; } = null!;

        public DateTime TokenExpiresIn { get; set; }

        public string TokenType { get; set; } = "Bearer";

        public string TokenExpiresMinutes { get; set; } = string.Empty;

        public string RefreshTokenExpiresMinutes { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
    }
}
