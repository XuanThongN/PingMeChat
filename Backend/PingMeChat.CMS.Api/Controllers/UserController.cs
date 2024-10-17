using System.Security.Claims;
using AutoMapper;
using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Record.Chart;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Attachments;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.Shared;
using PingMeChat.Shared.Utils;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        public UserController(IUserService userService, IMapper mapper, IAttachmentService attachmentService)
        {
            _userService = userService;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.User.PaginationRoute)]
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
        [Route(ApiRoutes.Feature.User.AddRoute)]
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
        [Route(ApiRoutes.Feature.User.GetAllActive)]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _userService.GetAllActive();
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "tài khoản người dùng đang hoạt động"), result, 200));
        }

        [HttpGet]
        [Route(ApiRoutes.Feature.User.GetByIdRoute)]
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
        [Route(ApiRoutes.Feature.User.UpdateRoute)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            var email = GetEmail();
            dto.UpdatedBy = email;
            var result = await _userService.Update(dto);

            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "tài khoản người dùng"), result, StatusCodes.Status200OK));
        }

        [HttpPost]
        [Route(ApiRoutes.Feature.User.UpdateFCMTokenRoute)]
        public async Task<IActionResult> UpdateFCMToken([FromBody] UpdateFCMTokenDto model)
        {
            var userId = GetUserId();
            var user = await _userService.FindById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var updateUserDto = new UserUpdateDto
            {
                Id = user.Id,
                FullName = user.FullName,
                // Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FCMToken = model.FCMToken
            };

            var result = await _userService.Update(updateUserDto);

            if (result != null)
            {
                return Ok(new ApiResponse("FCM token updated successfully", result, StatusCodes.Status400BadRequest));
            }

            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "tài khoản người dùng"), result, StatusCodes.Status200OK));
        }

        [HttpPut]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.User.UpdateProfileRoute)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = GetUserId();
            var user = await _userService.FindById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var updateUserDto = new UserUpdateDto
            {
                Id = user.Id,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                // Email = user.Email,
                UpdatedBy = userId
            };

            var result = await _userService.Update(updateUserDto);

            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "thông tin người dùng"), result, StatusCodes.Status200OK));
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.User.UpdateAvatarRoute)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAvatar([FromForm] UpdateAvatarDto input)
        {
            var userId = GetUserId();
            var user = await _userService.FindById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (input.avatar == null || input.avatar.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var uploadResult = await _attachmentService.UploadFileAsync(input.avatar);

            var updateUserDto = new UserUpdateDto
            {
                Id = user.Id,
                FullName = user.FullName!,
                // Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                AvatarUrl = uploadResult.Url,
                UpdatedBy = userId
            };

            var result = await _userService.Update(updateUserDto);

            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "avatar người dùng"), result, StatusCodes.Status200OK));
        }

        public class UpdateFCMTokenDto
        {
            public string FCMToken { get; set; }
        }
    }
}
