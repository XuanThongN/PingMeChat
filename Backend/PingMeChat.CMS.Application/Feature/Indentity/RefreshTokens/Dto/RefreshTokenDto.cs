using PingMeChat.CMS.Application.Common.Dto;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto
{
    public class RefreshTokenDto : ReadDto
    {
        public string TokenValue { get; set; }
        public string DeviceInfo { get; set; }
        public string IPAddress { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } // vô hiệu hóa refresh token mà k phải xóa
        public string AccountId { get; set; }
    }
}
