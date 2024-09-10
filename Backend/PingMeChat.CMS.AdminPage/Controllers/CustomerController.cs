using AutoWrapper.Models;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Customers.Dto;
using PingMeChat.CMS.Application.Feature.Service.Customers;
using PingMeChat.CMS.Application.Feature.Service.Customers.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NPOI.HSSF.Record.Chart;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
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
        public async Task<ApiResponse> Search([FromBody] CustomerSearchDto model)
        {
            var customerDtos = await _customerService.FindAll(x =>
                                                  (string.IsNullOrEmpty(model.Keyword) || x.PhoneNumber.Contains(model.Keyword)
                                                  || x.FullName.Contains(model.Keyword) || x.Address.Contains(model.Keyword)));
            return new ApiResponse("OK", customerDtos, StatusCodes.Status200OK);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
        {
            var customer = await _customerService.Find(match: x => x.PhoneNumber == dto.PhoneNumber);
            if (customer != null)
            {
                return Ok(new ApiResponse("Số điện thoại đã tồn tại", null, 400));
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _customerService.Add(dto);
            return Ok(new ApiResponse("Tạo khách hàng mới thành công", result, 201));
        }
        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] CustomerSearchDto model)
        {
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);
            var data = await _customerService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                    string.IsNullOrEmpty(model.Keyword) || x.PhoneNumber.Contains(model.Keyword)
                    || x.FullName.Contains(model.Keyword)
                    || x.Address.Contains(model.Keyword),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            var result = new ResultDataTable<CustomerDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách khách hàng", result, 200));
        }

        //Hiển thị form chỉnh sửa loại bàn bida
        public async Task<IActionResult> Update(string id)
        {
            var data = await _customerService.Find(match: t => t.Id == id) ?? throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            return PartialView("_Update", data);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CustomerUpdateDto dto)
        {
            var customer = await _customerService.Find(match: x => x.PhoneNumber == dto.PhoneNumber && x.Id != dto.Id);
            if (customer != null)
            {
                return Ok(new ApiResponse("Số điện thoại đã tồn tại", null, 400));
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            dto.UpdatedBy = email;
            var result = await _customerService.Update(dto);
            return Ok(new ApiResponse("Cập nhật Khách hàng thành công", result, 200));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Ok(new ApiResponse("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", null, 400));
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Ok(new ApiResponse("Không tìm thấy email tài khoản đang hoạt động"));
            }
            var result = await _customerService.Delete(id);
            if (result == null)
                return Ok(new ApiResponse("Xoá bàn thất bại", null, 500));
            return Ok(new ApiResponse("Đã xóa thành công", null, 200));
        }

        //Hiển thị chi tiết bàn bida
        public async Task<IActionResult> Detail(string id)
        {
            var data = await _customerService.Find(match: t => t.Id == id);
            if (data == null)
            {
                throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            }
            return PartialView("~/Views/Customer/_Detail.cshtml", data);
        }
    }
}
