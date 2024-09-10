using AutoMapper;
using AutoWrapper.Models;
using PingMeChat.CMS.AdminPage.Common.Filter;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Service;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.BidaTableSessionServicess.Dto;
using PingMeChat.CMS.Application.Feature.Service.Customers;
using PingMeChat.CMS.Application.Feature.Service.Orders;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Application.Feature.Service.SessionServicess;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    [Authorize]
    public class BidaTableController : BaseController
    {
        private readonly IBidaTableService _bidaTableService;
        private readonly IBidaTableTypeService _bidaTableTypeService;
        private readonly IBidaTableSessionServices _bidaTableSessionServices;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;
        //private readonly IToastNotification _toastNotification;

        public BidaTableController(IBidaTableService bidaTableService,
            IBidaTableTypeService bidaTableTypeService,
            IBidaTableSessionServices bidaTableSessionServices,
            IOrderService orderService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IAuthService authService,
            ICustomerService customerService
            //IToastNotification toastNotification
            )
        {
            _bidaTableService = bidaTableService;
            _bidaTableTypeService = bidaTableTypeService;
            _bidaTableSessionServices = bidaTableSessionServices;
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _customerService = customerService;
            //_toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.BidaTableType = await GetAllBidaTableType();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Pagination([FromBody] BidaTableSearchDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
            var email = this.User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");

            var route = Request.Path.Value ?? throw new AppException("Không tìm thấy route", StatusCodes.Status404NotFound);
            var pageRequest = new PaginationFilter(model.CurrentPage, model.Length);

            var data = await _bidaTableService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (string.IsNullOrEmpty(model.Keyword) || x.Code.Contains(model.Keyword))
                && (model.BidaTableStatus == null || x.BidaTableStatus == model.BidaTableStatus)
                && (string.IsNullOrEmpty(model.BidaTableTypeId) || x.BidaTableTypeId == model.BidaTableTypeId),
                include: inc => inc.Include(x => x.BidaTableType),
        orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            var result = new ResultDataTable<BidaTableDto>(model.Draw, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse("Danhh sách bàn bida", result, 200));


        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BidaTableCreateDto dto)
        {
            if (!ModelState.IsValid)
            {

                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _bidaTableService.Add(dto);

            return Ok(new ApiResponse("Tạo mới thành công", result, 201));
        }

        //Hiển thị form chỉnh sửa loại bàn bida
        public async Task<IActionResult> Update(string id)
        {
            var data = await _bidaTableService.Find(match: t => t.Id == id, include: inc => inc.Include(x => x.BidaTableType)) ?? throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            ViewBag.BidaTableType = await GetAllBidaTableType();
            return PartialView("_Update", data);
        }


        [HttpPut]
        public async Task<ApiResponse> Update([FromBody] BidaTableUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {

                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            dto.UpdatedBy = email;
            await _bidaTableService.Update(dto);

            return new ApiResponse("Cập nhật thành công", 200);
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
            var result = await _bidaTableService.Delete(id, email);
            if (!result)
                return Ok(new ApiResponse("Xoá bàn thất bại", null, 500));
            return  Ok(new ApiResponse("Đã xóa thành công", null, 200));
        }

        //Hiển thị chi tiết bàn bida
        public async Task<IActionResult> Detail(string id)
        {
            var data = await _bidaTableService.Find(match: t => t.Id == id, include: inc => inc.Include(x => x.BidaTableType));
            if (data == null)
            {
                throw new AppException("Không tìm thấy dữ liệu", StatusCodes.Status404NotFound);
            }
            return PartialView("~/Views/BidaTable/_Detail.cshtml", data);
        }

        [HttpPost]
        public async Task<ApiResponse> StartPlay([FromBody] StartPlayDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var startPlay = await _bidaTableService.StartPlay(model.BidaTableId, email);
            if (!startPlay) return new ApiResponse("Không thể bắt đầu phiên chơi", 400);
            return new ApiResponse("Phiên chơi bắt đầu tính giờ", 200);

        }

        [HttpPost]
        public async Task<IActionResult> Payment([FromBody] OrderPayDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var user = await _authService.Find(x => x.Email == email);

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        // Kết thúc phiên chơi
                        await _bidaTableSessionServices.EndSession(model.BidaTableId, email);

                        var bidaTable = await _bidaTableService.FindById(model.BidaTableId); // lấy thông tin bàn chơi
                        var orderParrentId = bidaTable.OrderParrentId; // lấy thông tin hóa đơn cha
                        model.OrderParentId = orderParrentId; // gán id hóa đơn cha vào model

                        // Tạo thông tin hóa đơn
                        var orderCreateDto = await CreateOrder(model, user.FullName, email, orderParrentId);

                        // Cập nhật lại trạng thái của bàn chơi và phiên chơi
                        await _bidaTableService.ChangeStatus(model.BidaTableId, email);
                        await _bidaTableSessionServices.ChangeStatus(model.BidaTableId, email);

                        // Xử lý tạo hóa đơn
                        var result = await ProcessOrder(orderCreateDto, model, email, orderParrentId);

                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return Ok(new ApiResponse("Ok", result, StatusCodes.Status200OK));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return Ok(new ApiResponse("Thanh toán không thành công. Vui lòng kiểm tra lại thông tin", null, StatusCodes.Status500InternalServerError));
                    }
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Debt([FromBody] DebtDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var user = await _authService.Find(x => x.Email == email);


            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var customerId = "";

                        if (!string.IsNullOrWhiteSpace(model.CustomerId))
                        {
                            var customer = await _customerService.FindById(model.CustomerId);
                            if (customer == null) new ApiResponse("Không tìm thấy khách hàng trong hệ thống", null, StatusCodes.Status400BadRequest);
                            customerId = model.CustomerId;
                        }
                        else if (model.Customer != null)
                        {
                            // tạo mới khách hàng vào hệ thống
                            var customer = await _customerService.Add(model.Customer);
                            customerId = customer.Id;
                        }
                        else
                        {
                            return Ok(new ApiResponse("Không tìm thấy khách hàng trong hệ thống", null, StatusCodes.Status400BadRequest));
                        }

                        // chấm dứt phiên chơi này
                        // Kết thúc phiên chơi
                        await _bidaTableSessionServices.EndSession(model.OrderPay.BidaTableId, email);

                        var bidaTable = await _bidaTableService.FindById(model.OrderPay.BidaTableId); // lấy thông tin bàn chơi
                        var orderParrentId = bidaTable.OrderParrentId; // lấy thông tin hóa đơn cha
                        model.OrderPay.OrderParentId = orderParrentId; // gán id hóa đơn cha vào model

                        // Tạo thông tin hóa đơn
                        var orderCreateDto = await CreateOrder(model.OrderPay, user.FullName, email, orderParrentId);
                        // set trạg thái hóa đơn này là đang nợ
                        model.OrderPay.Status = Status.Debt;
                        //// gán thông tin khách hàng vào bill nợ
                        model.OrderPay.CustomerId = customerId;

                        // cập nhật lại trạng thái ( rảnh) của bàn chơi
                        await _bidaTableService.ChangeStatus(model.OrderPay.BidaTableId, email);
                        await _bidaTableSessionServices.ChangeStatus(model.OrderPay.BidaTableId, email);

                        // Xử lý tạo hóa đơn
                        var result = await ProcessOrder(orderCreateDto, model.OrderPay, email, orderParrentId);

                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return Ok(new ApiResponse("Ok", result, StatusCodes.Status200OK));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return Ok(new ApiResponse("Ghi nợ không thành công. Vui lòng kiểm tra lại thông tin", null, StatusCodes.Status500InternalServerError));
                    }

                }
            });
        }
        // thêm sản phẩm, dịch vụ sử dụng trong phiên chơi này
        [HttpPost]
        public async Task<IActionResult> AddServiceSessionInSession([FromBody] ServiceInSessionDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var result = await _bidaTableSessionServices.UpdateServiceSession(model, email);
            if (result)
                return Ok(new ApiResponse("Cảm ơn bạn đã sử dụng thêm sản phẩm, dịch vụ", result, StatusCodes.Status200OK));
            return Ok(new ApiResponse("Thêm sản phẩm, dịch vụ không thành công", null, StatusCodes.Status500InternalServerError));
        }

        // lấy danh sách bàn đang trống
        public async Task<ApiResponse> GetEmptyTables()
        {
            var result = await _bidaTableService.GetEmptyTables();
            return new ApiResponse("Oki", result, StatusCodes.Status200OK);
        }

        // xác nhận chuyển bàn 
        public async Task<ApiResponse> ChangeTable([FromBody] ChangeTableDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
            OrderPayDto orderPayDto = new OrderPayDto
            {
                BidaTableId = model.BidaTableId
            };
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var user = await _authService.Find(x => x.Email == email);

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        // chấm dứt phiên chơi này
                        await _bidaTableSessionServices.EndSession(model.BidaTableId, email);
                        // lấy thông tin liên quan để tạo 1 hóa đơn
                        var orderCreateDto = await _orderService.CalculateOrderForPayment(orderPayDto, user.FullName);

                        // tạo mã hóa đơn
                        var code = await _orderService.GenerateUniqueCodeAsync();
                        orderCreateDto.CreatedBy = email;
                        orderCreateDto.UpdatedBy = email;
                        orderCreateDto.CreatedDate = DateTime.Now;
                        orderCreateDto.UpdatedDate = DateTime.Now;
                        orderCreateDto.Code = code;
                        orderCreateDto.Description = $"Chuyển bàn từ bàn #{orderCreateDto.BidaTableCode}";
                        orderCreateDto.Status = Status.Pending; // chuyển bàn thì sẽ chuyển sang trạng thái chờ xác nhận
                        // cập nhật lại trạng thái ( rảnh) của bàn chơi
                        var bidaTable = await _bidaTableService.FindById(model.BidaTableId);
                        var orderParrentId = bidaTable.OrderParrentId;
                        await _bidaTableService.ChangeStatus(model.BidaTableId, email);
                        await _bidaTableSessionServices.ChangeStatus(model.BidaTableId, email);
                        // tạo hóa đơn 
                        var result = await _orderService.CreateOrderForTable(orderCreateDto, orderParrentId, email);
                        // tạo phiên chơi mới
                        var startPlay = await _bidaTableService.StartPlayAfterChangeTable(model.BidaTableChangeId, result.ParrentId, email);
                        if (!startPlay) return new ApiResponse("Không thể bắt đầu phiên chơi", 400);
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return new ApiResponse("Ok", result, StatusCodes.Status200OK);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse("Chuyển bàn không thành công. Vui lòng kiểm tra lại thông tin", null, StatusCodes.Status500InternalServerError);
                    }

                }
            });
        }
        // - chuyển bàn

        public async Task<ApiResponse> SplitHour([FromBody] BidaTableIdDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new AppException("Thông tin yêu cầu không hợp lệ, vui lòng thử lại", 400);
            }
            OrderPayDto orderPayDto = new OrderPayDto
            {
                BidaTableId = model.BidaTableId
            };
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new AppException("Không tìm thấy email tài khoản đang hoạt động");
            var user = await _authService.Find(x => x.Email == email);

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        // chấm dứt phiên chơi này
                        await _bidaTableSessionServices.EndSession(model.BidaTableId, email);
                        // lấy thông tin liên quan để tạo 1 hóa đơn
                        var orderCreateDto = await _orderService.CalculateOrderForPayment(orderPayDto, user.FullName);

                        // tạo mã hóa đơn
                        var code = await _orderService.GenerateUniqueCodeAsync();
                        orderCreateDto.CreatedBy = email;
                        orderCreateDto.UpdatedBy = email;
                        orderCreateDto.CreatedDate = DateTime.Now;
                        orderCreateDto.UpdatedDate = DateTime.Now;
                        orderCreateDto.Code = code;
                        orderCreateDto.Description = "Tách giờ";
                        orderCreateDto.Status = Status.Pending; // chuyển bàn thì sẽ chuyển sang trạng thái chờ xác nhận

                        // cập nhật lại trạng thái ( rảnh) của bàn chơi
                        var bidaTable = await _bidaTableService.FindById(model.BidaTableId);
                        var orderParrentId = bidaTable.OrderParrentId;
                        await _bidaTableService.ChangeStatus(model.BidaTableId, email);
                        await _bidaTableSessionServices.ChangeStatus(model.BidaTableId, email);
                        // tạo hóa đơn 
                        var result = await _orderService.CreateOrderForTable(orderCreateDto, orderParrentId, email);
                        // tạo phiên chơi mới
                        var startPlay = await _bidaTableService.StartPlayAfterChangeHour(model.BidaTableId, result.ParrentId, email);
                        if (!startPlay) return new ApiResponse("Không thể bắt đầu phiên chơi", 400);
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return new ApiResponse("Ok", result, StatusCodes.Status200OK);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse("Chuyển bàn không thành công. Vui lòng kiểm tra lại thông tin", null, StatusCodes.Status500InternalServerError);
                    }

                }
            });
        }
        [HttpPost]
        public async Task<ApiResponse> GetAllServiceSession([FromBody] BidaTableIdDto model)
        {
            var data = await _bidaTableSessionServices.GetAllServiceSession(model.BidaTableId);
            return new ApiResponse("Ok", data, StatusCodes.Status200OK);
        }

        [HttpPost]
        public async Task<IActionResult> GetDetailTablePlaying([FromBody] BidaTableIdDto model)
        {
            var data = await _bidaTableSessionServices.GetAllTableAndServiceSession(model.BidaTableId);
            data.OrderHistories = await _orderService.GetRelativeBillData(data.ParentId);
            return PartialView("_DetailTablePlaying", data);
        }

        #region Các hàm hỗ trợ cho việc thanh toán/ghi nợ
        private async Task<OrderCreateDto> CreateOrder(OrderPayDto model, string userFullName, string email, string? orderParrentId)
        {
            var orderCreateDto = orderParrentId == null
                ? await _orderService.CalculateOrderForPayment(model, userFullName)
                : await _orderService.CalculateOrderForPaymentWhenHaveParentOrder(model, userFullName);

            orderCreateDto.Code = await _orderService.GenerateUniqueCodeAsync();
            orderCreateDto.CreatedBy = email;
            orderCreateDto.UpdatedBy = email;
            orderCreateDto.CreatedDate = DateTime.Now;
            orderCreateDto.UpdatedDate = DateTime.Now;

            return orderCreateDto;
        }

        private async Task<OrderParrentDto> ProcessOrder(OrderCreateDto orderCreateDto, OrderPayDto model, string email, string? orderParrentId)
        {
            OrderParrentDto result = new OrderParrentDto();

            if (orderParrentId == null)
            {
                orderCreateDto.TotalAmountAllOrder = orderCreateDto.TotalAmount;
                orderCreateDto.Status = model.Status;
                orderCreateDto.CustomerId = model.CustomerId;
                var orderDto = await _orderService.Add(orderCreateDto);
                result.OrderId = orderDto.Id;
            }
            else
            {
                await _orderService.CreateOrderForTable(orderCreateDto, orderParrentId, email);
                await _orderService.ApplyTotalAmountAllOrderForParentOrder(model); // cập nhật tổng tiền của tất cả các order
                await _orderService.ChangedStatusForAllOrder(orderParrentId, model.Status); // cập nhật trạng thái của tất cả các order
                result.OrderParrentId = orderParrentId;
            }

            return result;
        }
        #endregion

        #region BidaTableType
        private async Task<List<SelectListItem>> GetAllBidaTableType()
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
        #endregion
    }
}
