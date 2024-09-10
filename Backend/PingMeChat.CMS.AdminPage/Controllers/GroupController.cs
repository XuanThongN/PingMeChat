using AutoWrapper.Models;
using PingMeChat.CMS.AdminPage.Common.Filter;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Groups;
using PingMeChat.CMS.Application.Feature.Service.Groups.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] GroupSearchDto model)
        {
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _groupService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (string.IsNullOrEmpty(model.Keyword) || x.GroupName.Contains(model.Keyword)),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));
            var result = new ResultDataTable<GroupDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách nhóm người dùng", result, 200));
        }

        [HttpPost]
        [ValidateUserAndModel]
        public async Task<IActionResult> Create([FromBody] GroupCreateDto dto)
        {
            var email = GetEmail();
            var checkExitsName = await _groupService.CheckExitsName(dto.GroupName);
            if (checkExitsName)
            {
                return Ok(new ApiResponse("Tên nhóm quyền đã tồn tại", null, 400));
            }
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _groupService.Add(dto);

            return Ok(new ApiResponse("Tạo mới thành công", result, 201));
        }
        
        [HttpPut]
        [ValidateUserAndModel]
        public async Task<IActionResult> Update([FromBody] GroupUpdateDto dto)
        {
            var email = GetEmail();

            dto.UpdatedBy = email;
            var result = await _groupService.Update(dto);

            return Ok(new ApiResponse("Cập nhật thành công", result, 200));
        }
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            var data = await _groupService.FindById(id);
            if (data == null)
            {
                throw new AppException("Không tìm thấy nhóm người dùng theo yêu cầu", 404);
            }
            return Ok(new ApiResponse("Lấy thông tin nhóm người dùng thành công", data, 200));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _groupService.GetAllActive();

            return Ok(new ApiResponse("Danh sách nhóm quyền hoạt động", result, 200));
        }
    }
}
