using AutoWrapper.Models;
using PingMeChat.CMS.AdminPage.Common.Filter;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Roles;
using PingMeChat.CMS.Application.Feature.Service.Roles.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    public class RoleController : BaseController
    {
        private IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateUserAndModel]
        public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
        {
            var email = GetEmail();
            var checkExitsName = await _roleService.CheckExitsName(dto.Name);
            if (checkExitsName)
            {
                return Ok(new ApiResponse("Tên nhóm quyền đã tồn tại", null, 400));
            }
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _roleService.Add(dto);

            return Ok(new ApiResponse("Tạo mới nhóm quyền thành công", result, 201));
        }

        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] RoleSearchDto model)
        {
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _roleService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (string.IsNullOrEmpty(model.Keyword) || x.Name.Contains(model.Keyword)),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));
            var result = new ResultDataTable<RoleDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách nhóm người dùng", result, 200));
        }

        [HttpPut]
        [ValidateUserAndModel]
        public async Task<IActionResult> Update([FromBody] RoleUpdateDto dto)
        {
            var email = GetEmail();
            var checkExitsName = await _roleService.CheckExitsNameForUpdate(email, dto.Name);
            if (checkExitsName)
            {
                return Ok(new ApiResponse("Tên nhóm quyền đã tồn tại", null, 400));
            }
            dto.UpdatedBy = email;
            var result = await _roleService.Update(dto);

            return Ok(new ApiResponse("Cập nhật thành công", result, 200));
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            var data = await _roleService.FindById(id);
            if (data == null)
            {
                throw new AppException("Không tìm thấy nhóm quyền theo yêu cầu", 404);
            }
            return Ok(new ApiResponse("Lấy thông tin nhóm quyền thành công", data, 200));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _roleService.GetAllActive();

            return Ok(new ApiResponse("Danh sách nhóm quyền hoạt động", result, 200));
        }
    
    }
}
