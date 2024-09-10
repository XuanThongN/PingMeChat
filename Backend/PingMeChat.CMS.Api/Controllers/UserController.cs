using AutoWrapper.Models;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.Roles;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.Shared;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.User.PaginationRoute)]
        [ProducesResponseType(typeof(ResultDataTable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Pagination([FromBody] PaginationFilter<UserPaginationFilterDto> model)
        {
            var pageRequest = new PaginationFilter(model.PageNumber, model.PageSize);

            var data = await _userService.Pagination(
                 pageRequest.PageNumber,
                 pageRequest.PageSize,
                 predicate: x => (model.Data.IsLocked == null || x.IsLocked == model.Data.IsLocked)
                     &&
                     (
                         string.IsNullOrEmpty(model.Keyword) || x.UserName.Contains(model.Keyword) ||
                         string.IsNullOrEmpty(model.Keyword) || x.Email.Contains(model.Keyword) ||
                         string.IsNullOrEmpty(model.Keyword) || x.PhoneNumber.Contains(model.Keyword) ||
                         string.IsNullOrEmpty(model.Keyword) || x.FullName.Contains(model.Keyword)
                     ),
                 orderBy: ord => ord.OrderByDescending(x => x.CreatedDate)
             );

            var result = new ResultDataTable<UserDto>(pageRequest.PageNumber, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "tài khoản người dùng"), result, StatusCodes.Status200OK));
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.User.AddRoute)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            var email = GetEmail();
            dto.CreatedBy = email;
            dto.UpdatedBy = email;

            var result = await _userService.Add(dto);
            return Ok(new ApiResponse(string.Format(Message.Success.CreateCompleted, "tài khoản người dùng"), result, StatusCodes.Status201Created));
        }

        [HttpGet]
        [Route(RootPath.Feature.User.GetAllActive)]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _userService.GetAllActive();
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "tài khoản người dùng đang hoạt động"), result, 200));
        }

        [HttpGet]
        [Route(RootPath.Feature.User.GetByIdRoute)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException(Message.Warning.InvalidInfo, StatusCodes.Status404NotFound);
            }
            var data = await _userService.FindById(id);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedCompleted, "tài khoản người dùng"), data, StatusCodes.Status200OK));
        }

        [HttpPut]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.User.UpdateRoute)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            var email = GetEmail();
            dto.UpdatedBy = email;
            var result = await _userService.Update(dto);
            
            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "tài khoản người dùng"), result, StatusCodes.Status200OK));
        }
    }
}
