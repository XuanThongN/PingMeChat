using AutoWrapper.Models;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Roles;
using PingMeChat.CMS.Application.Feature.Service.Roles.Dto;
using PingMeChat.Shared;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        private IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.Role.PaginationRoute)]
        [ProducesResponseType(typeof(ResultDataTable<RoleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Pagination([FromBody] PaginationFilter<RolePaginationFilterDto> model)
        {
            var pageRequest = new PaginationFilter(model.PageNumber, model.PageSize);

            var data = await _roleService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (string.IsNullOrEmpty(model.Keyword) || x.Name.Contains(model.Keyword)),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));
            
            var result = new ResultDataTable<RoleDto>(pageRequest.PageNumber, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "nhóm quyền"), result, StatusCodes.Status200OK));
        }
        
        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.Role.AddRoute)]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
        {
            var email = GetEmail();
            var checkExitsName = await _roleService.CheckExitsName(dto.Name);
            if (checkExitsName)
            {
                return BadRequest(new ApiResponse(string.Format(Message.Error.IsExisted, "Tên nhóm quyền"), null, StatusCodes.Status400BadRequest));
            }
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _roleService.Add(dto);

            return Ok(new ApiResponse((string.Format(Message.Success.CreateCompleted, "tên nhóm quyền"), result, StatusCodes.Status201Created)));
        }

        [HttpPut]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.Role.UpdateRoute)]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] RoleUpdateDto dto)
        {
            var email = GetEmail();
            var checkExitsName = await _roleService.CheckExitsNameForUpdate(email, dto.Name);
            if (checkExitsName)
            {
                return BadRequest(new ApiResponse(string.Format(Message.Error.IsExisted, "Tên nhóm quyền"), null, StatusCodes.Status400BadRequest));

            }
            dto.UpdatedBy = email;
            var result = await _roleService.Update(dto);
            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "nhóm quyền"), result, StatusCodes.Status200OK));
        }

        [HttpGet]
        [Route(RootPath.Feature.Role.GetByIdRoute)]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            var data = await _roleService.FindById(id);
            if (data == null)
            {
                throw new AppException(Message.Warning.InvalidInfo, StatusCodes.Status404NotFound);
            }
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedCompleted, "nhóm quyền"), data, StatusCodes.Status200OK));

        }

        [HttpGet]
        [Route(RootPath.Feature.Role.GetAllActive)]
        [ProducesResponseType(typeof(List<RoleDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _roleService.GetAllActive();
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "nhóm quyền đang hoạt động"), result, StatusCodes.Status200OK));

        }
    }
}
