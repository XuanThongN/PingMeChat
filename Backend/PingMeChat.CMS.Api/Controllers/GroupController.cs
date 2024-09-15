using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.Api.Controllers;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Groups;
using PingMeChat.CMS.Application.Feature.Service.Groups.Dto;
using PingMeChat.Shared;
using PingMeChat.Shared.Utils;

namespace BIA.CMS.Api.Controllers
{
    [Authorize]
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService GroupService)
        {
            _groupService = GroupService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Group.PaginationRoute)]
        [ProducesResponseType(typeof(ResultDataTable<GroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Pagination([FromBody] PaginationFilter<GroupPaginationFilterDto> model)
        {
            var pageRequest = new PaginationFilter(model.PageNumber, model.PageSize);

            var data = await _groupService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x => (model.Data.Status == null || x.Status == model.Data.Status)
                && (string.IsNullOrEmpty(model.Data.GroupName) || x.GroupName.Contains(model.Data.GroupName)),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));
            
            var result = new ResultDataTable<GroupDto>(pageRequest.PageNumber, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "nhóm người dùng"), result, StatusCodes.Status200OK));
        }
        
        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Group.AddRoute)]
        [ProducesResponseType(typeof(GroupDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] GroupCreateDto dto)
        {
            var email = GetEmail();
            var checkExitsName = await _groupService.CheckExitsName(dto.GroupName);
            if (checkExitsName)
            {
                return BadRequest(new ApiResponse(string.Format(Message.Error.IsExisted, "Tên nhóm người dùng"), null, StatusCodes.Status400BadRequest));
            }
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _groupService.Add(dto);

            return Ok(new ApiResponse(string.Format(Message.Success.CreateCompleted, "Tên nhóm người dùng"), result, StatusCodes.Status201Created));
        }

        [HttpPut]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Group.UpdateRoute)]
        [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] GroupUpdateDto dto)
        {
            var email = GetEmail();
            var checkExitsName = await _groupService.CheckExitsNameForUpdate(email, dto.GroupName);
            if (checkExitsName)
            {
                return BadRequest(new ApiResponse(string.Format(Message.Error.IsExisted, "Tên nhóm người dùng"), null, StatusCodes.Status400BadRequest));

            }
            dto.UpdatedBy = email;
            var result = await _groupService.Update(dto);
            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "nhóm người dùng"), result, StatusCodes.Status200OK));
        }

        [HttpGet]
        [Route(ApiRoutes.Feature.Group.GetByIdRoute)]
        [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById( string id)
        {
            var data = await _groupService.FindById(id);
            if (data == null)
            {
                throw new AppException(Message.Warning.InvalidInfo, StatusCodes.Status404NotFound);
            }
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedCompleted, "nhóm người dùng"), data, StatusCodes.Status200OK));

        }

        [HttpGet]
        [Route(ApiRoutes.Feature.Group.GetAllActive)]
        [ProducesResponseType(typeof(List<GroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _groupService.GetAllActive();
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "nhóm người dùng đang hoạt động"), result, StatusCodes.Status200OK));

        }

        [HttpPut]
        [Route(ApiRoutes.Feature.Group.ChangeStatusRoute)]
        [ServiceFilter(typeof(InfoUserNotFoundFilter))]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException(Message.Warning.InvalidInfo, 400);
            }
            var email = GetEmail();
            var data = await _groupService.ChangeStatus(id, email);
            return Ok(new ApiResponse(string.Format(Message.Success.ChangeStatusCompleted, $"nhóm người dùng {data.GroupName}"), data, 200));
        }

        [HttpDelete]
        [ServiceFilter(typeof(UserExistsFilter))]
        [Route(ApiRoutes.Feature.Group.DeleteRoute)]
        [ProducesResponseType(typeof(IEnumerable<GroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse(string.Format(Message.Error.NotFound, "nhóm người dùng"), null, StatusCodes.Status400BadRequest));
            }

            var email = GetEmail();
            var data = await _groupService.Delete(id, email);

            return Ok(new ApiResponse(string.Format(Message.Success.DeletedCompleted, "nhóm người dùng"), data, StatusCodes.Status200OK));
        }


    }
}
