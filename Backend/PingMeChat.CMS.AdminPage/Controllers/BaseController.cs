using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using UAParser;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    public class BaseController : Controller
    {
        public string GetIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // Kiểm tra nếu request đi qua proxy hoặc load balancer
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Request.Headers["REMOTE_ADDR"].FirstOrDefault();
            }

            // Nếu là địa chỉ IPv6 loopback, chuyển thành IPv4 loopback
            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            // Kiểm tra tính hợp lệ của địa chỉ IP
            if (!IPAddress.TryParse(ipAddress, out _))
            {
                ipAddress = "Unknown";
            }

            return ipAddress;
        }

        public string GetDeviceInfo(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            return !string.IsNullOrEmpty(userAgent) ? userAgent : "Unknown";
        }

        public string GetDetailedDeviceInfo(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrEmpty(userAgent))
            {
                return "Unknown";
            }

            var uaParser = Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);

            var browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}";
            var os = $"{clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}";
            var device = clientInfo.Device.Family;

            return $"Browser: {browser}, OS: {os}, Device: {device}";
        }

        public string GetEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }
        public string GetRoute()
        {
            return Request.Path.Value;
        }
        public string GetUserId()
        {
            return User.FindFirstValue("UserId");

        }
        public string GetUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name);

        }
    }
}
