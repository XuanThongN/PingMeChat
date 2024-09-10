using AutoWrapper.Models;
using PingMeChat.CMS.AdminPage.Common.Filter;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    public class BidaTableTypeController : BaseController
    {
        private readonly IBidaTableTypeService _bidaTableTypeService;
        private readonly IToastNotification _toastNotification;
        public BidaTableTypeController(IBidaTableTypeService bidaTableTypeService, IToastNotification toastNotification)
        {
            _bidaTableTypeService = bidaTableTypeService;
            _toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ApiResponse> Create([FromBody] BidaTableTypeCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
            var existName = await _bidaTableTypeService.Find(x => x.Name.ToUpper().Equals(dto.Name.Trim().ToUpper()));
            if (existName != null)
            {
                return new ApiResponse("Tên loại bàn đã tồn tại", null, 400);
            }
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _bidaTableTypeService.Add(dto);
            if (result != null)
                // thông báo phía người dùng
                return new ApiResponse("Tạo mới thành công", result, 201);
            return new ApiResponse("Thêm mới thất bại", null, 400);

        }
        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] BidaTableTypeSearchDto model)
        {
            var email = this.User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _bidaTableTypeService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (string.IsNullOrEmpty(model.Keyword) || x.Name.Contains(model.Keyword)),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            var result = new ResultDataTable<BidaTableTypeDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách loại bàn bida", result, 200));
        }

        //Hiển thị chi tiết loại bàn bida
        public async Task<IActionResult> Detail(string id)
        {
            var data = await _bidaTableTypeService.FindById(id);
            if (data == null)
            {
                throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            }
            return PartialView("_Detail", data);
        }

        //Hiển thị form chỉnh sửa loại bàn bida
        public async Task<IActionResult> Edit(string id)
        {
            var data = await _bidaTableTypeService.FindById(id);
            if (data == null)
            {
                throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            }
            return PartialView("_Update", data);
        }

        //Chỉnh sửa loại bàn bida
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] BidaTableTypeUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var existName = await _bidaTableTypeService.Find(x => x.Name.ToUpper().Equals(dto.Name.Trim().ToUpper()));
            if (existName != null && existName.Id != dto.Id)
            {
                return Ok(new ApiResponse("Tên loại bàn đã tồn tại", null, 400));
            }
            dto.UpdatedBy = email;
            var result = await _bidaTableTypeService.Update(dto);
            if (result == null)
            {
                return Ok(new ApiResponse("Cập nhật thất bại", null, 400));
            }
            return Ok(new ApiResponse("Cập nhật thành công", result, 200));

        }

        //Xoá loại bàn bida 
        [HttpDelete]
        public async Task<ApiResponse> Delete([FromQuery] string id)
        {
            var result = await _bidaTableTypeService.Delete(id);
            if (result == null)
            {
                return new ApiResponse("Không tìm thấy dữ liệu", null, 404);
            }
            return new ApiResponse("Xoá thành công", null, 200);
        }

        [HttpGet]
        public async Task<List<SelectListItem>> GetAllBidaTableType()
        {
            var data = await _bidaTableTypeService.GetAll();

            if (data == null) return new List<SelectListItem>();

            return data.Select(e =>
            {
                return new SelectListItem()
                {
                    Value = e.Id,
                    Text = e.Name
                };
            }).ToList();
        }
    }
}
