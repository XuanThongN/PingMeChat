using AutoWrapper.Models;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.Customers.Dto;
using PingMeChat.CMS.Application.Feature.Service.Products;
using PingMeChat.CMS.Application.Feature.Service.Products.Dto;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] ProductSearchDto model)
        {
            var email = this.User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _productService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (string.IsNullOrEmpty(model.Keyword) || x.Code.Contains(model.Keyword) || x.Name.Contains(model.Keyword)),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            //return new ResultDataTable<ProductDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            var result = new ResultDataTable<ProductDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách sản phẩm, dịch vụ", result, 200));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            // Kiểm tra xem sản phẩm đã tồn tại chưa
            var product = await _productService.Find(match: x => x.Name.ToUpper().Equals(dto.Name.ToUpper()));
            if (product != null)
            {
                return Ok(new ApiResponse("Tên Sản phẩm/ Dịch vụ đã tồn tại", null, 400));
            }
            dto.Code = await _productService.GenerateUniqueCodeAsync();
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _productService.Add(dto);
            if (result == null)
            {
                return Ok(new ApiResponse("Tạo mới không thành công", null, 400));
            }
            return Ok(new ApiResponse("Tạo mới thành công",null, 201));
        }
        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Detail(string id)
        {
            var data = await _productService.FindById(id);
            if (data == null)
            {
                throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            }
            return PartialView("_Detail", data);
        }
        //Hiển thị form chỉnh sửa sản phẩm
        public async Task<IActionResult> Update(string id)
        {
            var data = await _productService.FindById(id);
            if (data == null)
            {
                throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            }
            return PartialView("_Update", data);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductUpdateDto dto)
        {
            var existProduct = await _productService.Find(match: x => x.Name == dto.Name && x.Id != dto.Id);
            if (existProduct != null)
            {
                return Ok(new ApiResponse("Tên sản phẩm đã tồn tại", null, 400));
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            dto.UpdatedBy = email;
            var result = await _productService.Update(dto);
            return Ok(new ApiResponse("Cập nhật Sản phẩm/Dịch vụ thành công", result, 200));
        }

        [HttpGet]
        public async Task<ApiResponse> GetAll()
        {
            var result = await _productService.GetAll();

            return new ApiResponse("oki", result, StatusCodes.Status200OK);
        }

        // chỉ lấy ra những sản phẩm còn số lượng trong kho
        [HttpGet]
        public async Task<ApiResponse> GetAllNotEmpty()
        {
            var result = await _productService.GetAllNotEmpty();

            return new ApiResponse("oki", result, StatusCodes.Status200OK);
        }



    }
}
