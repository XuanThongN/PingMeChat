using AutoWrapper.Models;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Exceptions;
using System.Security.Claims;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class AuthenController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthenController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route(ApiRoutes.Auth.LoginRoute)]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(ModelStateFilter))]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var ipAddress = GetIpAddress(HttpContext);
            var deviceInfo = GetDetailedDeviceInfo(HttpContext);
            var tokenDto = await _authService.Login(request, deviceInfo, ipAddress);
            // Thêm RefreshToken vào HttpOnly cookie
            //Response.Cookies.Append("RefreshToken", tokenDto.RefreshToken, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = false, // Sử dụng nếu bạn có HTTPS
            //   // SameSite = SameSiteMode.Strict, // bảo mật cao nhất cho cookie
            //                                    //  SameSite = SameSiteMode.None, // Cho phép cross-site access
            //    SameSite = SameSiteMode.Lax,
            //    Domain = "localhost", // hoặc domain thực tế
            //    Path = "/",
            //    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(tokenDto.RefreshTokenExpiresMinutes))
            //});

            return Ok(new ApiResponse(Message.Success.Auth.LoginCompleted, tokenDto, StatusCodes.Status200OK));
        }

        [HttpPut]
        [Route(ApiRoutes.Auth.LockAccountRoute)]
        [ServiceFilter(typeof(InfoUserNotFoundFilter))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> LockAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException(Message.Warning.InvalidInfo, 400);
            }
            var email = GetEmail();
            var data = await _authService.LockAccountById(id, email);
            if (!data) return Ok(new ApiResponse(Message.Error.LockAccountFail, data, 400));
            return Ok(new ApiResponse(Message.Success.Auth.LockAccountCompleted, data, 200));
        }

        [HttpPut]
        [Route(ApiRoutes.Auth.UnLockAccountRoute)]
        [ServiceFilter(typeof(InfoUserNotFoundFilter))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UnLockAccount(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException(Message.Warning.InvalidInfo, 400);
            }
            var email = GetEmail();
            var data = await _authService.UnlockAccount(id, email);
            if (!data) return Ok(new ApiResponse(Message.Error.UnLockAccountFail, data, 400));
            return Ok(new ApiResponse(Message.Success.Auth.UnLockAccountCompleted, data, 200));
        }

        [HttpPost]
        [Route(ApiRoutes.Auth.LogoutRoute)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var accessToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (accessToken == null)
            {
                return BadRequest(new ApiResponse(Message.Error.LogoutFail, null, StatusCodes.Status400BadRequest));
            }
            var data = await _authService.Logout(accessToken);
            if (!data) return BadRequest(new ApiResponse(Message.Error.LogoutFail, null, StatusCodes.Status400BadRequest));
            return Ok(new ApiResponse(Message.Success.Auth.LogoutCompleted, data, StatusCodes.Status200OK));
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Auth.ChangePasswordRoute)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var email = GetEmail();
          
           var data =  await _authService.ChangePassword(model, email);
            if(!data) return BadRequest(new ApiResponse(Message.Error.ChangePasswordFail, null, StatusCodes.Status400BadRequest));
            return Ok(new ApiResponse(Message.Success.Auth.ChangePasswordCompleted, data, 200));
        }
    }
}
