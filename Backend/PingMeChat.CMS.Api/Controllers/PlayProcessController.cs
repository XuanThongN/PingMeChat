using AutoWrapper.Models;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Service;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.Customers;
using PingMeChat.CMS.Application.Feature.Service.Customers.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Application.Feature.Service.SessionServicess;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class PlayProcessController : BaseController
    {
        private readonly IBidaTableService _bidaTableService;
        private readonly IBidaTableSessionServices _bidaTableSessionServices;
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;

        public PlayProcessController(IBidaTableService bidaTableService,
            IBidaTableSessionServices bidaTableSessionServices,
            IOrderService orderService,
            IUnitOfWork unitOfWork,
            IAuthService authService,
            ICustomerService customerService
            )
        {
            _bidaTableService = bidaTableService;
            _bidaTableSessionServices = bidaTableSessionServices;
            _orderService = orderService;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _customerService = customerService;
        }


        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.PlayProcess.StartPlayRoute)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> StartPlay([FromBody] StartPlayDto model)
        {
            var email = GetEmail();
            var startPlay = await _bidaTableService.StartPlay(model.BidaTableId, email);
            if (!startPlay) return BadRequest(new ApiResponse(Message.Error.PlayProcess.UnableToStart, 400));
            return Ok(new ApiResponse(Message.Success.PlayProcess.Start, 200));

        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.PlayProcess.PaymentRoute)]
        [ProducesResponseType(typeof(OrderParrentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Payment([FromBody] OrderPayDto model)
        {
            var email = GetEmail();
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
                        var orderCreateDto = await _orderService.CalculateOrderForPayment(model, user.FullName);

                        // tạo mã hóa đơn
                        var code = await _orderService.GenerateUniqueCodeAsync();
                        orderCreateDto.CreatedBy = email;
                        orderCreateDto.UpdatedBy = email;
                        orderCreateDto.CreatedDate = DateTime.Now;
                        orderCreateDto.UpdatedDate = DateTime.Now;
                        orderCreateDto.Code = code;
                        var bidaTable = await _bidaTableService.FindById(model.BidaTableId);
                        var orderParrentId = bidaTable.OrderParrentId;
                        // cập nhật lại trạng thái ( rảnh) của bàn chơi
                        await _bidaTableService.ChangeStatus(model.BidaTableId, email);
                        await _bidaTableSessionServices.ChangeStatus(model.BidaTableId, email);

                        OrderParrentDto result = new OrderParrentDto();
                        // tạo hóa đơn 
                        if (orderParrentId == null)
                        {
                            var orderDto = await _orderService.Add(orderCreateDto);
                            result.OrderId = orderDto.Id;
                        }
                        else
                        {
                            await _orderService.CreateOrderForTable(orderCreateDto, orderParrentId, email);
                            result.OrderParrentId = orderParrentId;
                        }
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return Ok(new ApiResponse(Message.Success.PlayProcess.Payment, result, StatusCodes.Status200OK));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }

                }
            });
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.PlayProcess.DebtRoute)]
        [ProducesResponseType(typeof(OrderParrentDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Debt([FromBody] DebtDto model)
        {
            var email = GetEmail();
            var user = await _authService.Find(x => x.Email == email);

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    CustomerDto customer = new CustomerDto();

                    // không truyền id
                    if (string.IsNullOrWhiteSpace(model.CustomerId))
                    {
                        // nếu k truyền id khách hàng thì xem có truyền dữ liệu để tạo k
                        if (model.Customer == null)
                        {
                            throw new AppException(string.Format(Message.Error.NotFound, "khách hàng"), StatusCodes.Status400BadRequest);
                        }
                        else
                        {
                            // tạo mới khách hàng vào hệ thống
                            customer = await _customerService.Add(model.Customer);
                        }
                    }
                    else
                    {
                        // truyền id
                        customer = await _customerService.FindById(model.CustomerId);
                        if (customer == null) // khônng tìm thấy khách hàng vs id
                        {
                            throw new AppException(string.Format(Message.Error.NotFound, "khách hàng"), StatusCodes.Status400BadRequest);
                        }
                    }

                    // chấm dứt phiên chơi này
                    await _bidaTableSessionServices.EndSession(model.OrderPay.BidaTableId, email);
                    // lấy thông tin liên quan để tạo 1 hóa đơn
                    var orderCreateDto = await _orderService.CalculateOrderForPayment(model.OrderPay, user.FullName);

                    // tạo mã hóa đơn
                    var code = await _orderService.GenerateUniqueCodeAsync();
                    orderCreateDto.CreatedBy = email;
                    orderCreateDto.UpdatedBy = email;
                    orderCreateDto.CreatedDate = DateTime.Now;
                    orderCreateDto.UpdatedDate = DateTime.Now;
                    orderCreateDto.Code = code;
                    var bidaTable = await _bidaTableService.FindById(model.OrderPay.BidaTableId);
                    var orderParrentId = bidaTable.OrderParrentId;
                    // cập nhật lại trạng thái ( rảnh) của bàn chơi
                    await _bidaTableService.ChangeStatus(model.OrderPay.BidaTableId, email);
                    await _bidaTableSessionServices.ChangeStatus(model.OrderPay.BidaTableId, email);

                    // set trạg thái hóa đơn này là đang nợ
                    orderCreateDto.Status = Status.Debt;
                    // gán thông tin khách hàng vào bill nợ
                    orderCreateDto.CustomerId = customer.Id;
                    var result = new OrderParrentDto();

                    // tạo hóa đơn
                    if (orderParrentId == null)
                    {
                        var orderDto = await _orderService.Add(orderCreateDto);
                        result.OrderId = orderDto.Id;
                    }
                    else
                    {
                        await _orderService.CreateOrderForTable(orderCreateDto, orderParrentId, email);
                        result.OrderParrentId = orderParrentId;
                    }
                    await _unitOfWork.SaveChangeAsync();
                    await transaction.CommitAsync();

                    return Ok(new ApiResponse(string.Format(Message.Success.PlayProcess.Debt, bidaTable.Code, customer.FullName), result, StatusCodes.Status200OK));
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.PlayProcess.AddProductSessionRoute)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddProductSessionInSession([FromBody] ServiceInSessionDto model)
        {
            var email = GetEmail();

            var result = await _bidaTableSessionServices.UpdateServiceSession(model, email);
            return Ok(new ApiResponse(Message.Success.PlayProcess.AddProductSession, result, StatusCodes.Status200OK));
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.PlayProcess.ChangeTableRoute)]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeTable([FromBody] ChangeTableDto model)
        {

            OrderPayDto orderPayDto = new OrderPayDto
            {
                BidaTableId = model.BidaTableId
            };
            var email = GetEmail();
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
                        orderCreateDto.Description = "";
                        // cập nhật lại trạng thái ( rảnh) của bàn chơi
                        var bidaTable = await _bidaTableService.FindById(model.BidaTableId);
                        var orderParrentId = bidaTable.OrderParrentId;
                        await _bidaTableService.ChangeStatus(model.BidaTableId, email);
                        await _bidaTableSessionServices.ChangeStatus(model.BidaTableId, email);
                        // tạo hóa đơn 
                        var result = await _orderService.CreateOrderForTable(orderCreateDto, orderParrentId, email);
                        // tạo phiên chơi mới
                        var startPlay = await _bidaTableService.StartPlayAfterChangeTable(model.BidaTableChangeId, result.ParrentId, email);
                        if (!startPlay) throw new AppException(Message.Error.PlayProcess.UnableToStart, 400);
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return Ok(new ApiResponse(Message.Success.PlayProcess.ChangeTable, result, StatusCodes.Status200OK));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }

                }
            });
        }


        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.PlayProcess.SplitHourRoute)]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> SplitHour([FromBody] BidaTableIdDto model)
        {

            OrderPayDto orderPayDto = new OrderPayDto
            {
                BidaTableId = model.BidaTableId
            };
            var email = GetEmail();
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
                        orderCreateDto.Description = "";
                        // cập nhật lại trạng thái ( rảnh) của bàn chơi
                        var bidaTable = await _bidaTableService.FindById(model.BidaTableId);
                        var orderParrentId = bidaTable.OrderParrentId;
                        await _bidaTableService.ChangeStatus(model.BidaTableId, email);
                        await _bidaTableSessionServices.ChangeStatus(model.BidaTableId, email);
                        // tạo hóa đơn 
                        var result = await _orderService.CreateOrderForTable(orderCreateDto, orderParrentId, email);
                        // tạo phiên chơi mới
                        var startPlay = await _bidaTableService.StartPlayAfterChangeHour(model.BidaTableId, result.ParrentId, email);
                        if (!startPlay) throw new AppException(Message.Error.PlayProcess.UnableToStart, 400);
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return Ok(new ApiResponse(Message.Success.PlayProcess.SplitHour, result, StatusCodes.Status200OK));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }

                }
            });
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.PlayProcess.GetAllProductSessionRoute)]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProductSession([FromBody] BidaTableIdDto model)
        {
            var data = await _bidaTableSessionServices.GetAllServiceSession(model.BidaTableId);
            return Ok(new ApiResponse(string.Format(Message.Success.PlayProcess.GetAllProductSession, data.Code), data, StatusCodes.Status200OK));
        }
    }
}
