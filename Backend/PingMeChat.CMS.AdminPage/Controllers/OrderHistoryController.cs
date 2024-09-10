using AutoWrapper.Models;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys;
using PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    public class OrderHistoryController : Controller
    {
        private readonly IOrderHistoryService _orderHistoryService;   
        public OrderHistoryController(IOrderHistoryService orderHistoryService)
        {
            _orderHistoryService = orderHistoryService;
        }
        [HttpGet]
        public IActionResult Index(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] OrderHistorySearchDto model)
        {
            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _orderHistoryService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                x.OrderId == model.OrderId,
                include: x => x.Include(x => x.Customer),
                orderBy: ord => ord.OrderByDescending(x => x.UpdatedDate));

            foreach (var orderDto in data.Data)
            {
                if (orderDto.ParrentId != null)
                {
                    var orders = await _orderHistoryService.FindAll(x => x.ParrentId == orderDto.ParrentId);
                    orderDto.TotalAmount = orders.Sum(x => x.TotalAmount);
                }
            }
            var result = new ResultDataTable<OrderHistoryDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách lịch sử hoá đơn", result, 200));
        }

        [HttpPost]
        public async Task<ApiResponse> GetById([FromBody] string id)
        {
            var orderHis = await _orderHistoryService.FindById(id);
            return new ApiResponse("Thông tin lịch sử hóa đơn", orderHis, StatusCodes.Status200OK);
        }
    }
}
