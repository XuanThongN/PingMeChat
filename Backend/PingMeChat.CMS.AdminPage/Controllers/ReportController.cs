using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Reports.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoWrapper.Models;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    [AllowAnonymous]

    public class ReportController : BaseController
    {

        private readonly IOrderService _orderService;

        public ReportController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Overview()
        {
           
            return View();
        }

        public IActionResult Invoice()
        {
            return View();
        }

        public IActionResult ReportDebt()
        {
            return View();
        }

        public IActionResult TaskList()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult>  GetDashboardOverview([FromBody] ReportSearchDto model){
            
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
          
            var data = await _orderService.GetDashboardOverview(model);

            return Ok(new ApiResponse("OKi", data, 200));


        }
        [HttpPost]
        public async Task<IActionResult> GetAllOrderInfo([FromBody] ReportSearchDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }

            var data = await _orderService.GetAllOrderInfo(model);
            var result = new ResultDataTable<ReportDetails>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);

            return Ok(new ApiResponse("OKi", result, 200));
        }
        [HttpPost]
        public async Task<IActionResult> CapacityUsingTable([FromBody]  ReportSearchDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }

            var data = await _orderService.CapacityUsingTable(model);
            var result = new ResultDataTable<ReportDetailsBida>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);

            return Ok(new ApiResponse("OKi", result, 200));
        }

        [HttpPost]
        public async Task<IActionResult> GetAllReportProduct([FromBody] ReportSearchDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }

            var data = await _orderService.GetAllReportProduct(model);
            var result = new ResultDataTable<ReportDetailsProduct>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);

            return Ok(new ApiResponse("OKi", result, 200));
        }

    }
}
