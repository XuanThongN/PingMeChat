using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Octokit;
using System.Security.Claims;

namespace PingMeChat.CMS.Application.Common.Middlewares
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenRefreshMiddleware> _logger;

        public TokenRefreshMiddleware(RequestDelegate next, ILogger<TokenRefreshMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context, IJwtLib jwtLib, ITokenService tokonService, IAccountRepository accountRepository)
        {
            // Kiểm tra xem endpoint có được đánh dấu là AllowAnonymous không
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            // Kiểm tra xem endpoint có phải là endpoint đặc biệt không cần xác thực
            if (IsSpecialEndpoint(context.Request.Path))
            {
                await _next(context);
                return;
            }
            // Lấy access token từ header Authorization và refresh token từ cookie
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var refreshToken = context.Request.Headers["RefreshToken"].FirstOrDefault()?.Split(" ").Last();
           // var refreshToken = context.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // Giải mã access token - nếu token hết hạn thì tiếp tục dùng thêm refresh token để tạo token mới
            var claimsPrincipal = jwtLib.GetPrincipalFromExpiredToken(accessToken);
            if (claimsPrincipal != null)
            {
                await _next(context);
                return;
            }
            
            try
            {
                

                // Kiểm tra refresh token có hợp lệ không - có tồn tại - đã bị hủy  trong hệ thống không
                var refreshTokenEntity = await tokonService.CheckValidRefreshtoken(refreshToken);
                if (refreshTokenEntity == null || refreshTokenEntity.AccountId == null || refreshTokenEntity.IsRevoked)
                {
                    _logger.LogWarning(string.Format(Message.Warning.InvalidRefreshTokenForUser, ""));
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                // Tìm user dựa trên account id trong refresh token
                var user = await accountRepository.FindById(refreshTokenEntity.AccountId);
                if (user == null)
                {
                    _logger.LogWarning(string.Format(Message.Warning.UserNotFoundForEmail, refreshTokenEntity.AccountId));
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                // Tạo access token mới
                var newAccessToken = jwtLib.GenerateToken(user, null);

                // Thay thế token cũ trong request
                context.Request.Headers["Authorization"] = $"Bearer {newAccessToken}";

                // Tùy chọn: Thêm token mới vào response header để client có thể cập nhật
                context.Response.Headers.Add("X-New-Access-Token", newAccessToken);
                _logger.LogInformation(string.Format(Message.Info.InFo03, user.Id));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Message.Error.Token.ErrorOccurred);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                throw;
            }

            await _next(context);
        }

        // Phương thức kiểm tra xem endpoint có phải là endpoint đặc biệt không
        private bool IsSpecialEndpoint(string path)
        {
            // Thêm các endpoint đặc biệt vào đây
            var specialEndpoints = new[] { "/api/auth/login",  "/api/auth/register",  "/swagger/index.html", "/swagger/v1/swagger.json" };
            return specialEndpoints.Any(e => path.StartsWith(e, StringComparison.OrdinalIgnoreCase));
        }
    }
}
