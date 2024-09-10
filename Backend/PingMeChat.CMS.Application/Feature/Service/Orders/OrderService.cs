using AutoMapper;
using AutoWrapper.Models;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.App.Repositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Helpers;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Domain.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Application.Feature.Service.Reports.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System.Collections.Generic;
using Status = PingMeChat.CMS.Entities.Feature.Status;

namespace PingMeChat.CMS.Application.Feature.Service.Orders
{
    public interface IOrderService : IServiceBase<Order, OrderCreateDto, OrderUpdateDto, OrderDto, IOrderRepository>
    {
        public Task<OrderCreateDto> CalculateOrderForPayment(OrderPayDto input, string staffName);
        public Task<OrderCreateDto> CalculateOrderForPaymentWhenHaveParentOrder(OrderPayDto input, string staffName);
        public Task<OrderDto> Cancel(OrderCancelDto input, string email);
        Task<string> GenerateUniqueCodeAsync();
        Task<OrderDto> CreateOrderForTable(OrderCreateDto model, string orderParrentId, string email);

        Task<List<OrderDto>> GetBillData(string? orderId, string? orderParrentId);
        Task<List<OrderDto>> GetRelativeBillData(string orderParrentId);

        Task<OrderDto> ChangedStatus(OrderIdDto model, string email);
        Task<OrderDto> UpdateServiceDetailsInBill(OrderDetailsUpdateDto model, string email);
        Task<OrderDto> UpdateServiceDetailsInBillAndChangeStatus(OrderDetailsUpdateDto model, string email);
     
        Task<bool> ApplyTotalAmountAllOrderForParentOrder(OrderPayDto input);
        Task<bool> ChangedStatusForAllOrder(string parentOrderId, Status status);
        Task<ReportDashboardOverviewDto> GetDashboardOverview(ReportSearchDto model);
        Task<PagedResponse<List<ReportDetails>>> GetAllOrderInfo(ReportSearchDto model);
        Task<PagedResponse<List<ReportDetailsBida>>> CapacityUsingTable(ReportSearchDto model);
        Task<PagedResponse<List<ReportDetailsProduct>>> GetAllReportProduct(ReportSearchDto model);
    }

    public class OrderService : ServiceBase<Order, OrderCreateDto, OrderUpdateDto, OrderDto, IOrderRepository>, IOrderService
    {
        private readonly IBidaTableRepository _bidaTableRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly IInventoryRepository _inventoryRepository;
        public OrderService(IOrderRepository orderRepository, 
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IUriService uriService, 
            IBidaTableRepository bidaTableRepository, 
            IOrderHistoryRepository orderHistoryRepository,
            IInventoryRepository inventoryRepository) : base(orderRepository, unitOfWork, mapper, uriService)
        {
            _bidaTableRepository = bidaTableRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<OrderDto> CreateOrderForTable(OrderCreateDto model, string orderParrentId, string email)
        {
            var data = _mapper.Map<Order>(model);
            data.ParrentId = orderParrentId ?? data.Id;
            data.CreatedBy = email;
            data.UpdatedBy = email;


            await _repository.Add(data);
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<OrderDto>(data);

        }
        public async Task<List<OrderDto>> GetBillData(string orderId, string orderParrentId)
        {
            List<OrderDto> list = new List<OrderDto>();
            if (orderParrentId != null)
            {
                var orders = await _repository.FindAll(x => x.ParrentId == orderParrentId);
                var orderDtos = _mapper.Map<List<OrderDto>>(orders.OrderBy(x => x.CreatedDate));
                list.AddRange(orderDtos);
            }
            else
            {
                var order = await _repository.FindById(orderId);
                var orderDto = _mapper.Map<OrderDto>(order);

                list.Add(orderDto);
            }

            return list;
        }
        public async Task<List<OrderDto>> GetRelativeBillData(string orderParrentId)
        {
            List<OrderDto> list = new List<OrderDto>();
            if (orderParrentId == null) return list;
            var orders = await _repository.FindAll(x => x.ParrentId == orderParrentId);
            var orderDtos = _mapper.Map<List<OrderDto>>(orders.OrderByDescending(x => x.CreatedDate));
            list.AddRange(orderDtos);
            return list;
        }
      
        public async Task<OrderDto> ChangedStatus(OrderIdDto model, string email)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var order = await _repository.FindById(model.Id);
                        if (order == null) return new OrderDto();

                        // tạo mới 1 bản ghi lịch sử thao tác trên order 
                        var orderHis = _mapper.Map<OrderHistory>(order);
                        await _orderHistoryRepository.Add(orderHis);

                        order.Status = model.Status;
                        order.UpdatedBy = email;

                        await _repository.Update(order);



                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return _mapper.Map<OrderDto>(order);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new OrderDto();

                        // nên bắn 1 throw exception ở đây
                    }

                }
            });


        }

        public async Task<OrderDto> UpdateServiceDetailsInBill(OrderDetailsUpdateDto model, string email)
        {
            var order = await _repository.Find(x => x.Id == model.Id);
            if (order == null) throw new AppException("Không tìm thấy hóa đơn theo yêu cầu", null, StatusCodes.Status400BadRequest);

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // Tạo mới 1 bản ghi lịch sử thao tác trên order 
                    var orderHis = _mapper.Map<OrderHistory>(order);
                    await _orderHistoryRepository.Add(orderHis);

                    // Lấy ra tổng tiền dịch vụ trước khi thay đổi
                    var totalServiceAmountBefore = CalculateTotalServiceAmount(order.OrderDetails);

                    // Kiểm tra kho và cập nhật số lượng
                    await UpdateInventoryAndOrderDetails(order, model.OrderDetails, email);

                    // Lấy ra tổng tiền dịch vụ sau khi thay đổi
                    var totalServiceAmountAfter = CalculateTotalServiceAmount(order.OrderDetails);

                    // Cập nhật lại tổng tiền trong bill
                    order.TotalAmount = order.BidaTableAmount + totalServiceAmountAfter;

                    // Cập nhật TotalAmountAllOrder cho order chính và các order con
                    await UpdateTotalAmountAllOrder(order);

                    order.UpdatedBy = email;
                    order.UpdatedDate = DateTime.Now;

                    await _repository.Update(order);
                    await _unitOfWork.SaveChangeAsync();
                    await transaction.CommitAsync();

                    return _mapper.Map<OrderDto>(order);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        private async Task UpdateInventoryAndOrderDetails(Order order, List<OrderDetailDto> newOrderDetails, string email)
        {
            var currentOrderDetails = order.OrderDetails ?? new List<OrderDetail>();
            var updatedOrderDetails = new List<OrderDetail>();

            foreach (var newDetail in newOrderDetails)
            {
                var existingDetail = currentOrderDetails.FirstOrDefault(d => d.ProductId == newDetail.ProductId);
                var inventory = await _inventoryRepository.Find(x => x.ProductId == newDetail.ProductId, include: p => p.Include(o => o.Product));

                if (inventory == null)
                {
                    throw new AppException($"Không tìm thấy sản phẩm trong kho: {newDetail.ProductName}", null, StatusCodes.Status400BadRequest);
                }

                int quantityDifference = newDetail.Quantity - (existingDetail?.Quantity ?? 0);

                if (inventory.Quantity < quantityDifference)
                {
                    throw new AppException($"Số lượng trong kho không đủ cho sản phẩm: {newDetail.ProductName}", null, StatusCodes.Status400BadRequest);
                }

                if (existingDetail != null)
                {
                    existingDetail.Quantity = newDetail.Quantity; // Số lượng
                    //existingDetail.UnitPrice = newDetail.UnitPrice; // Giá bán (không cập nhật giá bán)
                    existingDetail.SubTotal = newDetail.Quantity * existingDetail.UnitPrice; // Tổng tiền (cập nhật lại tổng tiền)
                    updatedOrderDetails.Add(existingDetail);
                }
                else
                {
                    var newOrderDetail = new OrderDetail
                    {
                        ProductId = inventory.Product.Id, // Id sản phẩm
                        ProductName = inventory.Product.Name, // Tên sản phẩm
                        Quantity = newDetail.Quantity, // Số lượng
                        UnitPrice = inventory.Product.Price, // Giá bán
                        SubTotal = newDetail.Quantity * inventory.Product.Price, // Tổng tiền
                        Cost = inventory.Product.Cost.Value // Giá nhập
                    };
                    updatedOrderDetails.Add(newOrderDetail);
                }

                // Cập nhật số lượng trong kho
                inventory.Quantity -= quantityDifference;
                await _inventoryRepository.Update(inventory);
            }

            // Xóa các chi tiết không còn trong danh sách mới
            foreach (var oldDetail in currentOrderDetails.Except(updatedOrderDetails))
            {
                var inventory = await _inventoryRepository.Find(x => x.ProductId == oldDetail.ProductId);
                if (inventory != null)
                {
                    inventory.Quantity += oldDetail.Quantity;
                    await _inventoryRepository.Update(inventory);
                }
            }

            order.OrderDetails = updatedOrderDetails;
        }
        private async Task UpdateTotalAmountAllOrder(Order order)
        {
            // Kiểm tra xem đây có phải là order chính không
            bool isMainOrder = order.ParrentId == order.Id;

            if (isMainOrder)
            {
                // Đây là order chính
                var childOrders = await _repository.FindAll(x => x.ParrentId == order.Id && x.Id != order.Id);
                decimal totalAmountAllOrder = order.TotalAmount;  

                // Cộng tổng tiền từ các order con
                totalAmountAllOrder += childOrders.Sum(o => o.TotalAmount);

                // Áp dụng giảm giá
                totalAmountAllOrder = ApplyDiscount(order, totalAmountAllOrder);

                // Cập nhật TotalAmountAllOrder cho order chính
                order.TotalAmountAllOrder = totalAmountAllOrder;
                await _repository.Update(order);
            }
            else if (string.IsNullOrEmpty(order.ParrentId))
            {
                // Đây là order single
                decimal totalAmountAllOrder = ApplyDiscount(order, order.TotalAmount);
                order.TotalAmountAllOrder = totalAmountAllOrder;
                await _repository.Update(order);
            }
            else
            {
                // Đây là order con, cập nhật order chính
                var parentOrder = await _repository.Find(x => x.Id == order.ParrentId);
                if (parentOrder != null)
                {
                    await UpdateTotalAmountAllOrder(parentOrder);
                }
            }
        }

        // Áp dụng chiết khấu cho order chính sau khi cập nhật chi tiết dịch vụ trong hóa đơn (tính cả order con)
        private decimal ApplyDiscount(Order order, decimal amount)
        {
            if (order.DiscountType.HasValue)
            {
                if (order.DiscountType == DiscountType.Amount && order.Discount.HasValue)
                {
                    amount = Math.Max(0, amount - order.Discount.Value);
                }
                else if (order.DiscountType == DiscountType.Percent && order.DiscountPercent.HasValue)
                {
                    amount *= (1 - (decimal)order.DiscountPercent.Value / 100);
                }
            }
            return amount;
        }
        public async Task<OrderDto> UpdateServiceDetailsInBillAndChangeStatus(OrderDetailsUpdateDto model, string email)
        {
            var order = await _repository.Find(x => x.Id == model.Id);
            if (order == null) throw new AppException("Không tìm thấy hóa đơn theo yêu cầu", null, StatusCodes.Status400BadRequest);

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // Tạo mới 1 bản ghi lịch sử thao tác trên order 
                    var orderHis = _mapper.Map<OrderHistory>(order);
                    await _orderHistoryRepository.Add(orderHis);

                    // Lấy ra tổng tiền dịch vụ trước khi thay đổi
                    var totalServiceAmountBefore = CalculateTotalServiceAmount(order.OrderDetails);

                    // Kiểm tra kho và cập nhật số lượng
                    await UpdateInventoryAndOrderDetails(order, model.OrderDetails, email);

                    // Lấy ra tổng tiền dịch vụ sau khi thay đổi
                    var totalServiceAmountAfter = CalculateTotalServiceAmount(order.OrderDetails);

                    // Cập nhật lại tổng tiền trong bill
                    order.TotalAmount = order.BidaTableAmount + totalServiceAmountAfter;

                    // Cập nhật TotalAmountAllOrder
                    await UpdateTotalAmountAllOrder(order);

                    // Cập nhật trạng thái của bill chính thành done
                    if (order.ParrentId != null && order.ParrentId != order.Id)
                    {
                        var parentOrder = await _repository.Find(x => x.Id == order.ParrentId);
                        if (parentOrder != null && parentOrder.Status != Status.Done)
                        {
                            parentOrder.Status = Status.Done;
                            parentOrder.UpdatedBy = email;
                            parentOrder.UpdatedDate = DateTime.Now;
                            await _repository.Update(parentOrder);
                        }
                    }

                    // Cập nhật trạng thái của order hiện tại
                    order.Status = Status.Done;
                    order.UpdatedBy = email;
                    order.UpdatedDate = DateTime.Now;

                    await _repository.Update(order);
                    await _unitOfWork.SaveChangeAsync();
                    await transaction.CommitAsync();

                    return _mapper.Map<OrderDto>(order);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ;
                }
            });
        }


        #region tạo đối tượng orderCreateDto
        public async Task<OrderCreateDto> CalculateOrderForPayment(OrderPayDto input, string staffName)
        {
            var bidaTable = await GetActiveBidaTable(input.BidaTableId);
            var bidaTableSession = bidaTable.BidaTableSessions?.OrderByDescending(s => s.StartTime).FirstOrDefault()
                ?? throw new Exception("BidaTableSession not found");

            int timePlay = CalculatePlayTime(bidaTableSession);
            var orderDetails = CreateOrderDetails(bidaTableSession.ServiceSessions);
            decimal totalServiceAmount = CalculateTotalServiceAmount(orderDetails);
            decimal bidaTableAmount;
            decimal totalAmount = CalculateTotalAmount(out bidaTableAmount, totalServiceAmount, bidaTable.BidaTableType.Price, timePlay);
            var order = CreateOrderDto(bidaTable.Code, bidaTableSession, orderDetails, bidaTableAmount, totalAmount, input, staffName);
            ApplyDiscount(order, input);

            return order;
        }

        public async Task<OrderCreateDto> CalculateOrderForPaymentWhenHaveParentOrder(OrderPayDto input, string staffName)
        {
            var bidaTable = await GetActiveBidaTable(input.BidaTableId);
            var bidaTableSession = bidaTable.BidaTableSessions?.OrderByDescending(s => s.StartTime).FirstOrDefault()
                ?? throw new Exception("BidaTableSession not found");

            int timePlay = CalculatePlayTime(bidaTableSession);
            var orderDetails = CreateOrderDetails(bidaTableSession.ServiceSessions);
            decimal totalServiceAmount = CalculateTotalServiceAmount(orderDetails);
            decimal bidaTableAmount;
            decimal totalAmount = CalculateTotalAmount(out bidaTableAmount, totalServiceAmount, bidaTable.BidaTableType.Price, timePlay);
            var order = CreateOrderDto(bidaTable.Code, bidaTableSession, orderDetails, bidaTableAmount, totalAmount, input, staffName);

            // áp dụng chiết khấu sẽ không được thực hiện ở order con mà chỉ thực hiện ở order chính
            // nên set lại giá trị chiết khấu = 0 cho order con
            order.DiscountPercent = 0;
            order.Discount = 0;
            return order;
        }

        public async Task<bool> ChangedStatusForAllOrder(string parentOrderId, Status status)
        {
            var orders = await _repository.FindAll(x => x.Id == parentOrderId || x.ParrentId == parentOrderId);

            if (orders.ToList().Count == 0)
            {
                throw new AppException("Không tìm thấy hóa đơn theo yêu cầu", null, StatusCodes.Status400BadRequest);
            }
            orders.ToList().ForEach(x => x.Status = status);
            var result = await _repository.UpdateRange(orders);
            return result.Any() ? true : false;
        }

        // áp dụng chiết khấu cho order chính
        public async Task<bool> ApplyTotalAmountAllOrderForParentOrder(OrderPayDto input)
        {
            var parentOrder = await _repository.Find(x => x.Id == input.OrderParentId)
                                    ?? throw new AppException("Không tìm thấy hóa đơn nào theo yêu cầu", null, StatusCodes.Status400BadRequest);
            var data = await _repository.FindAll(o => o.ParrentId == parentOrder.Id)
                ?? throw new AppException("Không tìm thấy hóa đơn con nào theo yêu cầu", null, StatusCodes.Status400BadRequest);
            var totalAmountAllOrder = data.Sum(x => x.TotalAmount);
            parentOrder.TotalAmountAllOrder = totalAmountAllOrder;
            parentOrder.DiscountPercent = input.DiscountPercent;
            parentOrder.Discount = input.DiscountNumber;
            parentOrder.DiscountType = input.DiscountType;
            parentOrder.UpdatedDate = DateTime.Now;
            parentOrder.PaymentMethod = input.PaymentMethod;
            // Set lại trạng thái của order chính và tên của khách hàng trường hợp ghi nợ
            parentOrder.Status = input.Status;
            parentOrder.CustomerId = input.CustomerId;
            ApplyDiscountForParentOrder(parentOrder, input);
            await _repository.Update(parentOrder);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }
        // lấy thông tin bàn đang chơi và thông tin liên quan
        private async Task<BidaTable> GetActiveBidaTable(string bidaTableId)
        {
            var bidaTable = await _bidaTableRepository.Find(
                match: x => x.Id == bidaTableId
                    && (x.BidaTableStatus == BidaTableStatus.Playing
                        || x.BidaTableStatus == BidaTableStatus.SplitHour
                        || x.BidaTableStatus == BidaTableStatus.ChangeTable)
                    && x.BidaTableSessions.Any(s => s.SessionStatus == SessionStatus.Playing),
                include: o => o.Include(t => t.BidaTableType)
                               .Include(x => x.BidaTableSessions)
                               .ThenInclude(x => x.ServiceSessions)
                               .ThenInclude(x => x.Product), asSplitQuery: true);

            return bidaTable ?? throw new Exception("BidaTable not found");
        }

        // tính thời gian chơi (phút)
        private int CalculatePlayTime(BidaTableSession session)
             => (int)session.EndTime.Value.Subtract(session.StartTime).TotalMinutes;

        private List<OrderDetail> CreateOrderDetails(IEnumerable<ServiceSession> serviceSessions)
                => serviceSessions.Select(x => new OrderDetail
                {
                    ProductName = x.Product.Name,
                    Quantity = x.Quantity,
                    UnitPrice = x.Product.Price,
                    SubTotal = x.Product.Price * x.Quantity,
                    ProductId = x.ProductId,
                    Cost = x.Product.Cost ?? 0,
                }).ToList();

        // tổng tiền dịch vụ
        private decimal CalculateTotalServiceAmount(List<OrderDetail>? orderDetails)
        {
            if (orderDetails == null || orderDetails.Count <= 0) return 0;
            return orderDetails.Sum(x => x.SubTotal);
        }


        // tổng tiền cần thanh toán ( chưa tính chiết khấu)
        private decimal CalculateTotalAmount(out decimal bidaTableAmount, decimal totalServiceAmount, decimal tablePrice, int timePlay)
        {
            bidaTableAmount = Math.Ceiling((decimal)(tablePrice * timePlay / 60) / 1000m) * 1000m;
            return totalServiceAmount + bidaTableAmount;
        }

        // tạo đối tượng OrderCreateDto
        private OrderCreateDto CreateOrderDto(string code, BidaTableSession session, List<OrderDetail> orderDetails, decimal bidaTableAmount, decimal totalAmount, OrderPayDto input, string staffName)
            => new OrderCreateDto
            {
                BidaTableCode = code,
                OrderDetails = orderDetails,
                BidaTableAmount = bidaTableAmount,
                TotalAmount = totalAmount,
                OrderDate = DateTime.Now,
                StartTime = session.StartTime,
                EndTime = session.EndTime.Value,
                Status = Status.Done,
                PaymentMethod = input.PaymentMethod,
                Description = input.Description,
                StaffName = staffName,
                DiscountPercent = input.DiscountPercent,
                DiscountType = input.DiscountType,
                Discount = input.DiscountNumber,
            };

        // áp dụng chiết khấu
        private void ApplyDiscount(OrderCreateDto order, OrderPayDto input)
        {
            if (input.DiscountNumber > 0 || input.DiscountPercent > 0)
            {
                order.DiscountType = input.DiscountType;
                order.Discount = input.DiscountType == DiscountType.Percent
                    ? order.TotalAmount * input.DiscountPercent / 100
                    : input.DiscountNumber;
                order.TotalAmount -= order.Discount;
            }
        }
        private void ApplyDiscountForParentOrder(Order order, OrderPayDto input)
        {
            if (input.DiscountNumber > 0 || input.DiscountPercent > 0)
            {
                order.DiscountType = input.DiscountType;
                order.Discount = input.DiscountType == DiscountType.Percent
                    ? order.TotalAmountAllOrder * input.DiscountPercent / 100
                    : input.DiscountNumber;
                order.TotalAmountAllOrder -= (decimal)order.Discount;
            }
        }

        #endregion
        public async Task<OrderDto> Cancel(OrderCancelDto input, string email)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var order = await _repository.FindById(input.OrderId);
                        if (order == null || order.Status == Status.Cancelled)
                            throw new AppException("Order not found or has been cancelled");
                        // Tạo 1 bảng lịch sử
                        var orderHis = _mapper.Map<OrderHistory>(order);
                        orderHis.CancelContent = input.CancelContent; // lý do huỷ
                        await _orderHistoryRepository.Add(orderHis);

                        order.UpdatedBy = email;
                        order.Status = Status.Cancelled;
                        order.CancelContent = input.CancelContent;
                        order.UpdatedDate = DateTime.Now;

                        await _repository.Update(order);
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return _mapper.Map<OrderDto>(order);
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        throw e;
                    }
                }
            });
        }

        #region tạo mã hóa đơn
        public async Task<string> GenerateUniqueCodeAsync()
        {
            string code;
            do
            {
                code = GenerateCode();
            } while (await CodeExistsAsync(code));

            return code;
        }

        private string GenerateCode()
        {
            // Format: ORD-YYMMDD-HHMMSS-XXXX
            // YY: năm, MM: tháng, DD: ngày, HH: giờ, MM: phút, SS: giây, XXXX: số ngẫu nhiên 4 chữ số
            var timestamp = DateTime.Now;
            var random = new Random();
            return $"ORD-{timestamp:yyMMdd-HHmmss}-{random.Next(1000, 9999):D4}";
        }

        private async Task<bool> CodeExistsAsync(string code)
        {
            return await _repository.AnyAsync(o => o.Code == code);
        }

        #endregion
        public async Task<ReportDashboardOverviewDto> GetDashboardOverview(ReportSearchDto model)
        {
            // lấy ra những order chính trong khoảng thời gian nào đó
            // nó chứa cả 3 trạng thái
            // và không tính tính order đã sinh những chưa thành toán ( TH này xảy ra cho những order tính bàn -tách giờ và chưa thanh toán)
            var orders = await _repository.FindAll(predicate: o => (o.ParrentId == null
                                                          || (o.ParrentId == o.Id && o.Status != Status.Pending))
                                                          && o.OrderDate >= model.StartDate
                                                          && o.OrderDate <= model.EndDate);

            // danh sách order hoàn thành;
            var doneOrders = orders.Where(x => x.Status == Status.Done).ToList();

            // danh sách order nợ
            var debtOrders = orders.Where(x => x.Status == Status.Debt).ToList();

            // danh sách order hủy
            var cancelOrders = orders.Where(x => x.Status == Status.Cancelled).ToList();

            var totalOrders = orders.Count();
            var totalOrderDone = doneOrders.Count();
            var totalOrderDebt = debtOrders.Count();
            var totalOrderCancel = cancelOrders.Count();

            // tổng vốn mua hàng trên tất cả thằng con + cha
            decimal totalPurchaseCostDone = 0;

            foreach (var item in doneOrders)
            {
                // TH tách bàn tách h
                if (item.Id == item.ParrentId)
                {
                    var itemChil = await _repository.FindAll(x => x.ParrentId == item.Id);
                    totalPurchaseCostDone += itemChil.Sum(x => x.OrderDetails != null ? x.OrderDetails.Sum(y => y.Quantity * y.Cost) : 0);

                }
                else
                {
                    totalPurchaseCostDone += item.OrderDetails != null ? item.OrderDetails.Sum(x => x.Cost * x.Quantity) : 0;
                }


            }

            // tổng vốn mua hàng trên tất cả thằng con + cha
            decimal totalPurchaseCostDebt = 0;

            foreach (var item in debtOrders)
            {
                // TH tách bàn tách h
                if (item.Id == item.ParrentId)
                {
                    var itemChil = await _repository.FindAll(x => x.ParrentId == item.Id);
                    totalPurchaseCostDebt += itemChil.Sum(x => x.OrderDetails != null ? x.OrderDetails.Sum(y => y.Quantity * y.Cost) : 0);

                }
                else
                {
                    totalPurchaseCostDebt += item.OrderDetails != null ? item.OrderDetails.Sum(x => x.Cost * x.Quantity) : 0;
                }


            }


            decimal totalCash = 0;
            decimal totalTranfer = 0;
            foreach (var item in doneOrders)
            {
                if (item.PaymentMethod == PaymentMethod.Cash)
                {
                    totalCash += item.TotalAmountAllOrder;
                }
                else if (item.PaymentMethod == PaymentMethod.BankTransfer)
                {
                    totalTranfer += item.TotalAmountAllOrder;
                }
            }

            // TH hoàn thành

            // tổng vốn mua hàng trên tất cả thằng cha
            //   decimal totalPurchaseCostDoneParent = doneOrders.Sum(x => x.OrderDetails != null ? x.OrderDetails.Sum(x => x.Quantity * x.Cost) : 0);

            // tổng vốn mua hàng
            //  decimal totalPurchaseCostDone = totalPurchaseCostDoneChild + totalPurchaseCostDoneParent;
            decimal totalDiscountDone = doneOrders.Sum(x => x.Discount ?? 0);
            decimal totalAmountAfterDiscountDone = doneOrders.Sum(x => x.TotalAmountAllOrder);
            decimal totalRevenueDone = totalAmountAfterDiscountDone + totalDiscountDone;
            decimal profitDone = totalAmountAfterDiscountDone - totalPurchaseCostDone;


            // TH nợ
            // decimal totalPurchaseCostDebt = debtOrders.Sum(x => x.OrderDetails != null ? x.OrderDetails.Sum(x => x.Quantity * x.Cost) : 0);
            decimal totalDiscountDebt = debtOrders.Sum(x => x.Discount ?? 0);
            decimal totalAmountAfterDiscountDebt = debtOrders.Sum(x => x.TotalAmountAllOrder);
            decimal totalRevenueDebt = totalAmountAfterDiscountDebt + totalDiscountDebt;
            decimal profitDebt = totalAmountAfterDiscountDebt - totalPurchaseCostDebt;

            ReportDashboardOverviewDto reportDashboardOverviewDto = new ReportDashboardOverviewDto
            {
                TotalOrders = totalOrders,
                TotalOrderDone = totalOrderDone,
                TotalOrderDebt = totalOrderDebt,
                TotalOrderCancel = totalOrderCancel,
                TotalPurchaseCostDone = totalPurchaseCostDone,
                TotalDiscountDone = totalDiscountDone,
                TotalAmountAfterDiscountDone = totalAmountAfterDiscountDone,
                TotalRevenueDone = totalRevenueDone,
                ProfitDone = profitDone,
                TotalPurchaseCostDebt = totalPurchaseCostDebt,
                TotalDiscountDebt = totalDiscountDebt,
                TotalAmountAfterDiscountDebt = totalAmountAfterDiscountDebt,
                TotalRevenueDebt = totalRevenueDebt,
                ProfitDebt = profitDebt,
                TotalCash = totalCash,
                TotalTransfer = totalTranfer
            };

            return reportDashboardOverviewDto;
        }

        public async Task<PagedResponse<List<ReportDetails>>> GetAllOrderInfo(ReportSearchDto model)
        {

            // lấy ra những order chính trong khoảng thời gian nào đó
            // kết hợp với đk trạng thái
            // và không tính tính order đã sinh những chưa thành toán ( TH này xảy ra cho những order tính bàn -tách giờ và chưa thanh toán)
            var listData = await _repository.Pagination(out int total,
                                                        model.CurrentPage,
                                                        model.Length,
                                                        predicate: x => x.Status == model.Status
                                                        && (x.ParrentId == null
                                                        || (x.ParrentId == x.Id && x.Status != Status.Pending))
                                                        && x.OrderDate >= model.StartDate
                                                        && x.OrderDate <= model.EndDate,
                                                        orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            List<ReportDetails> orderDetails = new List<ReportDetails>();
            // tổng doanh thu bán trên tất cả thằng con + cha
            //   decimal secondaryRevenue = 0;
            foreach (var item in listData)
            {
                ReportDetails reportDetail = new ReportDetails();

                // TH tách bàn tách h
                if (item.Id == item.ParrentId)
                {
                    var itemChil = await _repository.FindAll(x => x.ParrentId == item.Id);
                    reportDetail.MainRevenue = itemChil.Sum(x => x.BidaTableAmount);
                    reportDetail.SecondaryRevenue = itemChil.Sum(x => x.OrderDetails != null ? x.OrderDetails.Sum(y => y.Quantity * y.UnitPrice) : 0);
                    reportDetail.Expense = itemChil.Sum(x => x.OrderDetails != null ? x.OrderDetails.Sum(y => y.Quantity * y.Cost) : 0);
                }
                else
                {
                    reportDetail.MainRevenue = item.BidaTableAmount;
                    reportDetail.SecondaryRevenue = item.OrderDetails != null ? item.OrderDetails.Sum(x => x.Quantity * x.UnitPrice) : 0;
                    reportDetail.Expense = item.OrderDetails != null ? item.OrderDetails.Sum(x => x.Quantity * x.Cost) : 0;

                }

                reportDetail.Date = item.OrderDate;
                //reportDetail.MainRevenue = item.BidaTableAmount;
                reportDetail.Discount = item.Discount ?? 0;
                reportDetail.GrossProfit = reportDetail.MainRevenue + reportDetail.SecondaryRevenue - reportDetail.Discount - reportDetail.Expense;
                reportDetail.StatusName = item.Status.GetDescription();
                reportDetail.Status = item.Status;

                orderDetails.Add(reportDetail);
            }

            return PaginationHelper.CreatePagedReponse(orderDetails, new PaginationQuery(model.CurrentPage, model.Length), total, _uriService, null);

        }

        public async Task<PagedResponse<List<ReportDetailsBida>>> CapacityUsingTable(ReportSearchDto model)
        {
            // lấy ra tất cả order trong khoảng thời gian nào đó
            // có cả order chính và order phụ

            var totalTimeObject = await _repository.FindAllSelect(
                                         predicate: o => o.Status != Status.Pending && o.OrderDate >= model.StartDate && o.OrderDate <= model.EndDate,
                                         selector: x => new { x.BidaTableCode, Duration = (x.EndTime - x.StartTime).TotalMinutes });

            // Nhóm và tính tổng thời gian sử dụng cho từng mã bàn bida
            var groupedDurations = totalTimeObject.GroupBy(x => x.BidaTableCode)
                                                  .Select(g => new
                                                  {
                                                      TableCode = g.Key,
                                                      TotalDuration = g.Sum(x => x.Duration)
                                                  }).ToList();


            var data = await _bidaTableRepository.Pagination(out int total,
                                              pageNumber: model.CurrentPage,
                                              pageSize: model.Length,
                                                null,
                                              include: x => x.Include(x => x.BidaTableType),
                                              orderBy: x => x.OrderBy(x => x.BidaTableType.Name)
                                          );

            List<ReportDetailsBida> reportDetailsBidas = new List<ReportDetailsBida>();
            foreach (var item in data)
            {
                var totalDuration = groupedDurations
                    .Where(g => g.TableCode == item.Code)
                    .Select(g => g.TotalDuration)
                    .FirstOrDefault();

                reportDetailsBidas.Add(new ReportDetailsBida
                {
                    BidaTableId = item.Id,
                    BidaTableCode = item.Code,
                    BidaTableTypeName = item.BidaTableType.Name,
                    Price = item.BidaTableType.Price,
                    TotalTime = totalDuration,
                    ProductivityUsing = 0
                });
            }

            return PaginationHelper.CreatePagedReponse(reportDetailsBidas, new PaginationQuery(model.CurrentPage, model.Length), total, _uriService, null);

        }

        public async Task<PagedResponse<List<ReportDetailsProduct>>> GetAllReportProduct(ReportSearchDto model)
        {
            // lấy ra tất cả orderdetails trong khoảng thời gian nào đó
            // dựa trên order chính và order phụ
            var listOrderDetails = await _repository.FindAllSelect(
                                         predicate: o => o.Status != Status.Pending
                                         && o.OrderDate >= model.StartDate
                                         && o.OrderDate <= model.EndDate,
                                         selector: x => x.OrderDetails);

            List<ReportDetailsProduct> reportDetailsProducts = new List<ReportDetailsProduct>();

            foreach (var listItem in listOrderDetails)
            {
                if (listItem != null)
                {
                    foreach (var item in listItem)
                    {
                        //  ReportDetailsProduct reportDetailsProduct = new ReportDetailsProduct();

                        // nếu tồn tại thì cập nhật thêm các giá trị tương ứng trong reportDetailsProducts
                        if (reportDetailsProducts.Any(x => x.ProductId == item.ProductId))
                        {
                            var existingProduct = reportDetailsProducts.First(x => x.ProductId == item.ProductId);
                            existingProduct.Revenue += item.SubTotal;
                            existingProduct.Expense += item.Cost * item.Quantity;
                            existingProduct.GrossProfit = existingProduct.Revenue - existingProduct.Expense;
                            existingProduct.Quanlity += item.Quantity;
                        }
                        else // nếu chưa tồn tại tạo mới
                        {
                            reportDetailsProducts.Add(new ReportDetailsProduct
                            {
                                ProductId = item.ProductId,
                                ProductName = item.ProductName,
                                Revenue = item.SubTotal,
                                Expense = item.Cost * item.Quantity,
                                GrossProfit = item.SubTotal - (item.Cost * item.Quantity),
                                Quanlity = item.Quantity
                            });
                        }
                    }
                }

            }

            var total = reportDetailsProducts.Count();

            foreach (var item in reportDetailsProducts)
            {
                // số lượng hiện tại của sản phẩm có trong kho
                var productInInventory = await _inventoryRepository.Find(x => x.ProductId == item.ProductId);
                item.QuanlityAfter = productInInventory != null ? productInInventory.Quantity : 0;
                item.QuanlityBefore = item.QuanlityAfter + item.Quanlity;
            }
            // Áp dụng phân trang
            var pagedReportDetailsProducts = reportDetailsProducts
                .Skip((model.CurrentPage - 1) * model.Length)
                .Take(model.Length)
                .ToList();

            return PaginationHelper.CreatePagedReponse(reportDetailsProducts, new PaginationQuery(model.CurrentPage, model.Length), total, _uriService, null);

        }


    }
}
