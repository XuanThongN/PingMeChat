using AutoWrapper.Models;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.Inventories.Dto;
using PingMeChat.CMS.Application.Feature.Service.Inventorys;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers.Warehouse
{
    [AllowAnonymous]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
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
        public async Task<IActionResult> Pagination([FromBody] InventorySearchDto model)
        {
            var email = this.User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _inventoryService.Pagination(
                                 pageRequest.PageNumber,
                                 pageRequest.PageSize,
                                 predicate: x =>
                                     (string.IsNullOrEmpty(model.Keyword) || x.Product.Name.Contains(model.Keyword)),
                                 include: x => x.Include(i => i.Product),
                                 orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            var result = new ResultDataTable<InventoryDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách sản phẩm tồn kho", result, 200));
        }

        // Hiển thị chi tiết tồn kho
        public async Task<IActionResult> Detail(string id)
        {
            var data = await _inventoryService.Find(x => x.Id == id, include: inc => inc.Include(x => x.Product));
            if (data == null)
            {
                throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            }
            return PartialView("~/Views/Inventory/_Detail.cshtml", data);
        }

        public async Task<IActionResult> GetAll()
        {
            var result = await _inventoryService.GetAllInclude(x => x.Product);

            return Ok(new ApiResponse("oki", result, StatusCodes.Status200OK));
        }

    }
}
