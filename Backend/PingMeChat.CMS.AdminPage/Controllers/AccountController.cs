using AutoWrapper.Models;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Office.Interop.Word;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(
           IAuthService authService,
            ILogger<AccountController> logger
           )
        {
            _authService = authService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl; // Lưu URL trước đó trong ViewData

            return View();
        }
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Register()
        //{
        //    if (User.Identity.IsAuthenticated)
        //        return RedirectToAction("Index", "Home");


        //    return View();
        //}


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new AppException("Thông tin không hợp lệ");

                await SignIn(loginDto);

                return RedirectToLocal(returnUrl);

            }
            catch (AppException e)
            {
                ModelState.AddModelError("Error", e.Message);
                return View(loginDto);
            }
            catch (AggregateException ae)
            {
                ModelState.AddModelError("Error", "Tài khoản hoặc mật khẩu không chính xác");
                return View(loginDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đăng nhập không thành công");
                ModelState.AddModelError("Error", "Đăng nhập không thành công, vui lòng thử lại");
                return View(loginDto);
            }
        }
        /// <summary>
        /// Chức năng đăng ký tài khaonr người dùng bởi quản trị viên
        /// </summary>
        /// <param name="loginDto"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
     
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        ////[Authorize]
        //public async Task<IActionResult> Register(RegisterDto model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            throw new AppException("Vui lòng kiểm tra lại thông tin đã nhập", 400);

        //        //var email = User.FindFirstValue(ClaimTypes.Email);
        //        //if (email == null)
        //        //    return BadRequest(new ApiResponse("Không tìm thấy tài khoản đang hoạt động", 404));
        //        //var token = await _authService.Register(model, email);
        //        var token = await _authService.Register(model);

        //        return RedirectToAction("Login");

        //    }
        //    catch (AppException e)
        //    {
        //        ModelState.AddModelError("Error", e.Message);
        //        return View(model);
        //    }
        //    catch (AggregateException ae)
        //    {
        //        ModelState.AddModelError("Error", "Tài khoản hoặc mật khẩu không chính xác");
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("Error", "Đăng nhập không thành công, vui lòng thử lại");
        //        return View(model);
        //    }
        //}

        private async System.Threading.Tasks.Task SignIn(LoginDto request)
        {

            var ipAddress = GetIpAddress(HttpContext);
            var deviceInfo = GetDetailedDeviceInfo(HttpContext);
            var tokenDto = await _authService.Login(request, deviceInfo, ipAddress);
            var accessToken = tokenDto.AccessToken;

            // giải mã token để lưu dữ liệu đăng nhập vào phiên
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = Settings.JWT_ValidAudience,
                ValidIssuer = Settings.JWT_ValidIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.JWT_Secret))
            };

            var claims = DecodeToken(accessToken, validationParameters);

            // sau khi giải mã xong, tạo ra 1 token mới lưu nó vào cookie

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

            var tokenExpiresMinutes = Settings.JWT_TokenExpiresMinutes;

            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true, // nếu trình duyệt của người dùng tắt thì nó vẫn lưu lại phiên đăng nhập - không xóa cookie
                ExpiresUtc = DateTime.Now.AddMinutes(tokenExpiresMinutes)
            };

            await HttpContext.SignInAsync(scheme: JwtBearerDefaults.AuthenticationScheme, principal, authenticationProperties);
        }
        private List<Claim> DecodeToken(string token, TokenValidationParameters validationParameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>();

            try
            {
                // Giải mã token và lấy ra các Claim
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                claims = ((JwtSecurityToken)validatedToken).Claims.ToList();
            }
            catch (SecurityTokenValidationException)
            {
                // Xử lý lỗi khi token không hợp lệ
                // Thông báo, ghi log, hoặc xử lý theo ý muốn
            }
            catch (Exception)
            {
                // Xử lý lỗi chung khi giải mã token
                // Thông báo, ghi log, hoặc xử lý theo ý muốn
            }

            return claims;
        }
        /// <summary>
        /// returnUrl có phải là URL cục bộ hay không và nếu đúng như vậy,
        ///  tôi sẽ chuyển hướng người dùng đến địa chỉ đó,
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //if (!string.IsNullOrEmpty(accessToken))
            //{
            //    await _authService.Logout(accessToken);
            //}

            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequestDto model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null) 
                return BadRequest(new ApiResponse("Không tìm thấy tài khoản đang hoạt động", 404));
            await _authService.ChangePassword(model, email);
            return Ok(new ApiResponse("Đổi mật khẩu thành công", null, 200));
        }

        [HttpPost]
        public async Task<ApiResponse> VerifyPassword([FromBody] VerifyPasswordDto model)
        {
            if (!ModelState.IsValid) return new ApiResponse("Mật khẩu không hợp lệ", null, StatusCodes.Status400BadRequest);
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var checkVerifyPassword = await _authService.VerifyPassword(email, model.Password);
            if (!checkVerifyPassword) return new ApiResponse("Mật khẩu không chính xác", null, StatusCodes.Status404NotFound);
            return new ApiResponse("Ok", null, StatusCodes.Status200OK);
        }

        [HttpPost]
        public async Task<ApiResponse> UpdateAccountInfo([FromBody] UpdateAccountInfoDto model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse("Vui lòng kiểm tra lại thông tin", null, StatusCodes.Status400BadRequest);
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) 
                return new ApiResponse("Tài khoản người dùng không hợp lệ", null, StatusCodes.Status400BadRequest);

            var checkUpdate = await _authService.UpdateInfoAndPassword(model, email);
            if (!checkUpdate) 
                return new ApiResponse("Cập nhật thông tin không thành công, vui lòng thử lại", checkUpdate, StatusCodes.Status500InternalServerError);
            return new ApiResponse("Cập nhật thông tin thành công", checkUpdate, StatusCodes.Status200OK);

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return BadRequest( new ApiResponse("Tài khoản người dùng không hợp lệ", null, StatusCodes.Status400BadRequest));

            var result = await _authService.Find(x => x.Email == email);
            return Ok(new ApiResponse("Thông tin người dùng", result, StatusCodes.Status200OK));

        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> LockAccount(string userId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return BadRequest(new ApiResponse("Tài khoản người dùng không hợp lệ", null, StatusCodes.Status400BadRequest));

            await _authService.LockAccount(userId, email);
            return Ok(new ApiResponse("Khóa tài khoản thành công", null, StatusCodes.Status200OK));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnlockAccount(string userId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return BadRequest(new ApiResponse("Tài khoản người dùng không hợp lệ", null, StatusCodes.Status400BadRequest));

            await _authService.UnlockAccount(userId, email);
            return Ok(new ApiResponse("Mở khóa tài khoản thành công", null, StatusCodes.Status200OK));
        }

    }
}
