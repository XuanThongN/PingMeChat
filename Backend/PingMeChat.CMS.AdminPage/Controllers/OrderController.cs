using AutoWrapper.Models;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IToastNotification _toastNotification;
        private readonly IAuthService _authService;
        public OrderController(IOrderService orderService, IToastNotification toastNotification, IAuthService authService)
        {
            _orderService = orderService;
            _toastNotification = toastNotification;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Users = await _authService.GetSelectListUser();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] OrderSearchDto model)
        {
            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _orderService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (x.ParrentId == x.Id || x.ParrentId == null)
                &&
                 (string.IsNullOrEmpty(model.Keyword)
                        || x.Code.ToLower().Contains(model.Keyword.ToLower())
                        || x.Customer.PhoneNumber.ToLower().Contains(model.Keyword.ToLower())
                        || x.Customer.FullName.ToLower().Contains(model.Keyword.ToLower()))
                    && (string.IsNullOrEmpty(model.StaffName) || x.StaffName.ToLower().Equals(model.StaffName.ToLower()))
                    && (model.OrderStatus == null || x.Status == model.OrderStatus)
                    && (model.DiscountType == null || x.DiscountType == model.DiscountType)
                    && (model.PaymentMethod == null || x.PaymentMethod == model.PaymentMethod)
                    && (!model.StartTime.HasValue ||
                        x.OrderDate >= model.StartTime.Value)  // So sánh đến phút
                    && (!model.EndTime.HasValue ||
                        x.OrderDate <= model.EndTime.Value)  // So sánh đến phút
                    && x.Status != Status.Pending  // không lấy những hóa đơn chờ xác nhận
                    ,
                include: x => x.Include(x => x.Customer),
                orderBy: ord => ord.OrderByDescending(x => x.OrderDate));

            var result = new ResultDataTable<OrderDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách hóa đơn", result, 200));
        }

        [HttpPost]
        public async Task<ApiResponse> ChangedStatus([FromBody] OrderIdDto model)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse("Vui lòng kiếm tra lại thông tin", null, StatusCodes.Status400BadRequest);
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var result = await _orderService.ChangedStatus(model, email);
            return new ApiResponse($"Thay đổi trạng thái cho hoá đơn {result.Code} thành công", result, StatusCodes.Status200OK);
        }

        //Hàm huỷ hoá đơn với đầu vào là lý do huỷ hoá đơn
        [HttpPost]
        public async Task<ApiResponse> CancelOrder([FromBody] OrderCancelDto input)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse("Vui lòng kiếm tra lại thông tin", null, StatusCodes.Status400BadRequest);
            }
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var result = await _orderService.Cancel(input, email);

            return new ApiResponse($"Hủy hóa đơn {result.Code} thành công", result, StatusCodes.Status200OK);
        }

        [HttpGet]
        public async Task<IActionResult> BillView(string OrderId, string OrderParrentId)
        {
            // Xử lý logic để lấy dữ liệu cho hóa đơn
            var billData = await _orderService.GetBillData(OrderId, OrderParrentId);
            if (billData == null)
            {
                return RedirectToAction("Error404", "PageError");
            }

            // Trả về view với dữ liệu
            return View(billData);
        }

        [HttpPost]
        public async Task<ApiResponse> GetById([FromBody] OrderParrentDto model)
        {
            var result = await _orderService.GetBillData(model.OrderId, model.OrderParrentId);

            return new ApiResponse("Thông tin hóa đơn", result, StatusCodes.Status200OK);
        }

        /// <summary>
        /// Cập nhật lại số lượng sản phẩm ở trên hóa đơn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateServiceDetailsInBill([FromBody] OrderDetailsUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse("Vui lòng kiếm tra lại thông tin", null, StatusCodes.Status400BadRequest));
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var result = await _orderService.UpdateServiceDetailsInBill(model, email);

            return Ok(new ApiResponse($"Cập nhật hóa đơn {result.Code} thành công", result, StatusCodes.Status200OK));
        }

        /// <summary>
        /// Cập nhật lại số lượng sản phẩm ở trên hóa đơn và thay đổi trạng thái thành hoàn thành trên bill chính
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse> UpdateServiceDetailsInBillAndChangeStatus([FromBody] OrderDetailsUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse("Vui lòng kiếm tra lại thông tin", null, StatusCodes.Status400BadRequest);
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var result = await _orderService.UpdateServiceDetailsInBillAndChangeStatus(model, email);

            return new ApiResponse($"Cập nhật hóa đơn {result.Code} thành công", result, StatusCodes.Status200OK);
        }


    }
}
