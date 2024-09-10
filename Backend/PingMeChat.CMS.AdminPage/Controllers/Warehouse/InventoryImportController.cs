using AutoWrapper.Models;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.InventoryImports;
using PingMeChat.CMS.Application.Feature.Service.InventoryImports.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers.Warehouse
{
    [AllowAnonymous]
    public class InventoryImportController : Controller
    {
        private readonly IInventoryImportService _inventoryImportService;

        public InventoryImportController(IInventoryImportService inventoryImportService)
        {
            _inventoryImportService = inventoryImportService;
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
        public async Task<ApiResponse> Create([FromBody] InventoryImportCreateDto dto)
        {
            if (!ModelState.IsValid)
            {

                return new ApiResponse("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }

            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

                var result = await _inventoryImportService.Add(dto, email);

                return new ApiResponse("Tạo phiếu nhập kho thành công", 200);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse("Có lỗi xảy ra, vui lòng thử lại", 500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] InventoryImportSearchDto model)
        {
            var email = this.User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _inventoryImportService.Pagination(
                                 pageRequest.PageNumber,
                                 pageRequest.PageSize,
                                 predicate: x =>
                                     (string.IsNullOrEmpty(model.Keyword) || x.Code.Contains(model.Keyword) || x.PartnerName.Contains(model.Keyword))
                                     && (!model.InventoryType.HasValue || model.InventoryType.Value == x.InventoryType)
                                     && (!model.StartTime.HasValue || x.CreatedDate.Value.Date >= model.StartTime.Value.Date)
                                     && (!model.EndTime.HasValue || x.CreatedDate.Value.Date <= model.EndTime.Value.Date)
                                     ,
                                 orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            var result = new ResultDataTable<InventoryImportDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách hóa đơn nhập kho", result, 200));
        }

        //Chi tiết phiếu nhập kho
        public async Task<IActionResult> Detail(string id)
        {
            var inventoryImport = await _inventoryImportService.FindById(id);
            if (inventoryImport == null)
            {
                return NotFound(new ApiResponse("Không tìm thấy phiếu nhập kho", null, 404));
            }

            return PartialView("~/Views/InventoryImport/_Detail.cshtml", inventoryImport);
        }
    }
}
