using PingMeChat.CMS.Application;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.Shared.Utils;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Common.Middleware
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
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var refreshToken = context.Request.Headers["RefreshToken"];
            
            if(!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {

                var claimsPrincipal = jwtLib.GetPrincipalFromExpiredToken(accessToken);
                if (claimsPrincipal == null)
                {
                    _logger.LogWarning(Message.Warning.InvalidAccessToken);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                
                try
                {
                   
                    var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
                    if (string.IsNullOrEmpty(email))
                    {
                        _logger.LogWarning(Message.Warning.EmailClaimNotFound);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                    var user = await accountRepository.Find(x => x.Email == email);
                    if (user == null)
                    {
                        _logger.LogWarning(string.Format(Message.Warning.UserNotFoundForEmail, email));
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    var refreshTokenModel = await tokonService.GetSavedRefreshToken(user.Id);
                    if (refreshTokenModel == null || refreshTokenModel.TokenValue != refreshToken || refreshTokenModel.IsRevoked)
                    {
                        _logger.LogWarning(string.Format(Message.Warning.InvalidRefreshTokenForUser, user.Id));
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }

                    var newAccessToken = jwtLib.GenerateToken(user, null);
                    context.Response.Headers.Add("X-New-Access-Token", newAccessToken);
                    _logger.LogInformation(string.Format(Message.Info.InFo03, user.Id));
                   
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, Message.Error.Token.ErrorOccurred);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    throw;
                }
                
            }

            await _next(context);
        }
    }
}
