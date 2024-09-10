using AutoWrapper.Models;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.InventoryExports;
using PingMeChat.CMS.Application.Feature.Service.InventoryExports.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers.Warehouse
{
    [AllowAnonymous]
    public class InventoryExportController : Controller
    {
        private readonly IInventoryExportService _inventoryExportService;

        public InventoryExportController(IInventoryExportService inventoryExportService)
        {
            _inventoryExportService = inventoryExportService;
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
        public async Task<ApiResponse> Create([FromBody] InventoryExportCreateDto dto)
        {
            if (!ModelState.IsValid)
            {

                return new ApiResponse("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }

            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

                var result = await _inventoryExportService.Add(dto, email);

                return new ApiResponse("Tạo phiếu nhập kho thành công", 200);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse("Có lỗi xảy ra, vui lòng thử lại", 500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] InventoryExportSearchDto model)
        {
            var email = this.User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _inventoryExportService.Pagination(
                                 pageRequest.PageNumber,
                                 pageRequest.PageSize,
                                 predicate: x =>
                                     (string.IsNullOrEmpty(model.Keyword) || x.Code.Contains(model.Keyword) || x.PartnerName.Contains(model.Keyword))
                                     && (!model.InventoryType.HasValue || model.InventoryType.Value == x.InventoryType)
                                     && (!model.StartTime.HasValue || x.CreatedDate.Value.Date >= model.StartTime.Value.Date)
                                     && (!model.EndTime.HasValue || x.CreatedDate.Value.Date <= model.EndTime.Value.Date),
                                 orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            var result = new ResultDataTable<InventoryExportDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách hóa đơn xuất kho", result, 200));
        }

        // Hiển thị chi tiết tồn kho
        public async Task<IActionResult> Detail(string id)
        {
            var data = await _inventoryExportService.FindById(id);
            if (data == null)
            {
                throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            }
            return PartialView("~/Views/InventoryExport/_Detail.cshtml", data);
        }

    }
}
