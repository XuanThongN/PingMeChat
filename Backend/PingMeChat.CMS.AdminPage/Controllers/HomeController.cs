using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using PingMeChat.Shared.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    //[Authorize(Policy = PermissionNames.Admin.Page_Home)]
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IBidaTableService _bidaTableService;

        public HomeController(IBidaTableService bidaTableService)
        {

            _bidaTableService = bidaTableService;

        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var model = new BidaTableSearchDto { PageNumber = page, Length = 200 }; // default 200 item
            var result = await Pagination(model);
            ViewBag.StatusCounts = await _bidaTableService.GetTotalWithStatus();
            return View(result.Data);
        }

        private async Task<ResultDataTable<BidaTableDto>> Pagination([FromQuery] BidaTableSearchDto model)
        {
            var email = this.User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(1, 999);
            var test = await _bidaTableService.GetAll();

            var data = await _bidaTableService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (string.IsNullOrEmpty(model.Code) || x.Code.Contains(model.Code))
                && (model.BidaTableStatus == null || x.BidaTableStatus == model.BidaTableStatus)
                && (string.IsNullOrEmpty(model.BidaTableTypeId) || x.BidaTableTypeId == model.BidaTableTypeId),

                include: inc => inc.Include(x => x.BidaTableType)
                                .Include(x => x.BidaTableSessions),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            return new ResultDataTable<BidaTableDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);


        }

        public IActionResult BillView()
        {
            return View();
        }

    }
}
