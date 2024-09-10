using AutoWrapper.Models;
using PingMeChat.CMS.AdminPage.Common.Filter;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Service.Roles;
using PingMeChat.CMS.Application.Feature.Service.Roles.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAuthService _authService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService,
            IRoleService roleService,
            IAuthService authService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _authService = authService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAll();
            var roleResponse = roles.Select(x => x.Name).ToList();
            ViewBag.Roles = roleResponse;
            return View(roleResponse);
        }
        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] UserSearchDto model)
        {
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _userService.Pagination(
                 pageRequest.PageNumber,
                 pageRequest.PageSize,
                 predicate: x => (model.IsLocked == null || x.IsLocked == model.IsLocked)
                     &&
                     (
                         string.IsNullOrEmpty(model.Keyword) || x.UserName.Contains(model.Keyword) ||
                         string.IsNullOrEmpty(model.Keyword) || x.Email.Contains(model.Keyword) ||
                         string.IsNullOrEmpty(model.Keyword) || x.PhoneNumber.Contains(model.Keyword) ||
                         string.IsNullOrEmpty(model.Keyword) || x.FullName.Contains(model.Keyword)
                     ),
                 orderBy: ord => ord.OrderByDescending(x => x.CreatedDate)
             );

            var result = new ResultDataTable<UserDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách nhóm người dùng", result, 200));

        }

        [HttpGet]
        [ServiceFilter(typeof(InfoUserNotFoundFilter))]
        public async Task<IActionResult> LockAccount([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException("Id không hợp lệ", 400);
            }
            var email = GetEmail();
            var data = await _authService.LockAccountById(id, email);
            if (!data) return Ok(new ApiResponse("Khóa tài khoản không thành công", 400));
            return Ok(new ApiResponse("Đã khóa tài khoản thành công", data, 200));
        }

        [HttpGet]
        [ServiceFilter(typeof(InfoUserNotFoundFilter))]
        public async Task<IActionResult> UnLockAccount([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException("Id không hợp lệ", 400);
            }
            var email = GetEmail();
            var data = await _authService.UnlockAccount(id, email);
            if (!data) return Ok(new ApiResponse("Mở tài khoản không thành công", 400));
            return Ok(new ApiResponse("Đã mở khóa tài khoản thành công", data, 200));
        }

        [HttpPost]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {

            var email = GetEmail();
            dto.CreatedBy = email;
            dto.UpdatedBy = email;

            var result = await _userService.Add(dto);
            return Ok(new ApiResponse("Tạo mới tài khoản thành công", result, 201));

        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _userService.GetAllActive();

            return Ok(new ApiResponse("Danh sách nhóm quyền hoạt động", result, 200));
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException("Id không hợp lệ", 400);
            }
            var data = await _userService.FindById(id);
            return Ok(new ApiResponse("Thông tin tài khoản", data, 200));
        }
        [HttpPut]
        [ValidateUserAndModel]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            var email = GetEmail();
            dto.UpdatedBy = email;

            var result = await _userService.Update(dto);
            return Ok(new ApiResponse("Cập nhật tài khoản thành công", result, 200));
        }
    }
}
