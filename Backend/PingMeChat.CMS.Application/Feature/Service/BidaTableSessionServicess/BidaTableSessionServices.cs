using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.BidaTableSessionServicess.Dto;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Application.Feature.Service.SessionServices;
using PingMeChat.CMS.Application.Feature.Service.SessionServices.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.Shared;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.SessionServicess
{
    public interface IBidaTableSessionServices : IServiceBase<BidaTableSession, BidaTableSessionCreateDto, BidaTableSessionUpdateDto, BidaTableSessionDto, IBidaTableSessionRepository>
    {
        Task EndSession(string bidaTableId, string email);
        Task<bool> ChangeStatus(string bidaTableId, string email);
        Task<ServiceSessionPaymentDto> GetAllServiceSession(string bidaTableId);
        Task<List<SessionServiceDto>> GetAllByTableId(string bidaTableId);
        Task<bool> UpdateServiceSession(ServiceInSessionDto model, string email);
        Task<BidaTableSessionPaymentDto> GetAllTableAndServiceSession(string bidaTableId);
    }
    public class BidaTableSessionServices : ServiceBase<BidaTableSession, BidaTableSessionCreateDto, BidaTableSessionUpdateDto, BidaTableSessionDto, IBidaTableSessionRepository>, IBidaTableSessionServices
    {
        private readonly IServiceSessionRepository _serviceSessionRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryRepository _inventoryRepository;
        public BidaTableSessionServices(IBidaTableSessionRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IServiceSessionRepository serviceSessionRepository,
            IProductRepository productRepository,
            IInventoryRepository inventoryRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _serviceSessionRepository = serviceSessionRepository;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task EndSession(string bidaTableId, string email)
        {
            var bidaTableSession = await _repository.Find(match: x => x.BidaTableId == bidaTableId && x.SessionStatus == SessionStatus.Playing, orderBy: x => x.OrderByDescending(x => x.CreatedDate));
            if (bidaTableSession == null) throw new ApplicationException("Không tìm thấy phiên chơi ");

            bidaTableSession.EndTime = DateTime.Now.RoundToMinute();
            //bidaTableSession.SessionStatus = SessionStatus.Ended;
            bidaTableSession.UpdatedBy = email;

            await _repository.Update(bidaTableSession);
            await _unitOfWork.SaveChangeAsync();
        }
        public async Task<bool> ChangeStatus(string bidaTableId, string email)
        {
            var bidaTableSession = await _repository.Find(match: x => x.BidaTableId == bidaTableId && x.SessionStatus == SessionStatus.Playing, orderBy: x => x.OrderByDescending(x => x.CreatedDate), asNoTracking: false);
            if (bidaTableSession == null) throw new ApplicationException("Không tìm thấy phiên chơi ");

            bidaTableSession.SessionStatus = SessionStatus.Available;
            bidaTableSession.UpdatedBy = email;

            await _repository.Update(bidaTableSession);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }

        public async Task<ServiceSessionPaymentDto> GetAllServiceSession(string bidaTableId)
        {

            var bidaTableSession = await _repository.Find(match: x => x.BidaTableId == bidaTableId
                                                          && x.SessionStatus == SessionStatus.Playing,

                                                          include: inc => inc.Include(x => x.BidaTable)
                                                                              .ThenInclude(x => x.BidaTableType)
                                                                              .Include(x => x.ServiceSessions)
                                                                              .ThenInclude(x => x.Product),
                                                          orderBy: x => x.OrderByDescending(x => x.CreatedDate));

            if (bidaTableSession == null) return new ServiceSessionPaymentDto();

            // tổng thời gian chơi - tạm tính
            var price = bidaTableSession.BidaTable.BidaTableType.Price;
            var totalTime = (int)DateTime.Now.Subtract(bidaTableSession.StartTime).TotalMinutes;
            var totalTimeString = totalTime.ConvertMinutesToTimeString();
            // tổng tiền bàn
            var totalPrice = Math.Ceiling((decimal)(totalTime * price / 60) / 1000m) * 1000m;

            // danh sách sản phẩm đã dùng
            // tổng tiền dịch vụ sử dụng 
            decimal totalServicePrice = 0;
            List<SessionServiceDto> orderDetails = new List<SessionServiceDto>();


            if (bidaTableSession.ServiceSessions.Any())
            {
                totalServicePrice = bidaTableSession.ServiceSessions.Sum(x => x.Quantity * x.Price);
                orderDetails = _mapper.Map<List<SessionServiceDto>>(bidaTableSession.ServiceSessions);
            }

            var serviceSessionPaymentDto = new ServiceSessionPaymentDto
            {
                Code = bidaTableSession.BidaTable.Code,
                Name = bidaTableSession.BidaTable.BidaTableType.Name,
                Price = price,
                TotalTime = totalTime,
                TotalTimeString = totalTimeString,
                DiscountType = DiscountType.Percent,
                TotalServicePrice = totalServicePrice,
                SessionServiceDtos = orderDetails,
                TotalPrice = totalPrice,
            };

            return serviceSessionPaymentDto;
        }
        
        public async Task<BidaTableSessionPaymentDto> GetAllTableAndServiceSession(string bidaTableId)
        {

            var bidaTableSession = await _repository.Find(match: x => x.BidaTableId == bidaTableId
                                                          && x.SessionStatus == SessionStatus.Playing,

                                                          include: inc => inc.Include(x => x.BidaTable)
                                                                              .ThenInclude(x => x.BidaTableType)
                                                                              .Include(x => x.ServiceSessions)
                                                                              .ThenInclude(x => x.Product),
                                                          orderBy: x => x.OrderByDescending(x => x.CreatedDate));

            if (bidaTableSession == null) return new BidaTableSessionPaymentDto();

            var parentId = bidaTableSession.BidaTable.OrderParrentId;
            // tổng thời gian chơi - tạm tính
            var price = bidaTableSession.BidaTable.BidaTableType.Price;
            var startTime = bidaTableSession.StartTime;
            var endTime = DateTime.Now.RoundToMinute();
            var totalTime = (int)DateTime.Now.Subtract(startTime).TotalMinutes;
            var totalTimeString = totalTime.ConvertMinutesToTimeString();
            // tổng tiền bàn
            var totalPrice = Math.Ceiling((decimal)(totalTime * price / 60) / 1000m) * 1000m;

            // danh sách sản phẩm đã dùng
            // tổng tiền dịch vụ sử dụng 
            decimal totalServicePrice = 0;
            List<SessionServiceDto> orderDetails = new List<SessionServiceDto>();


            if (bidaTableSession.ServiceSessions.Any())
            {
                totalServicePrice = bidaTableSession.ServiceSessions.Sum(x => x.Quantity * x.Price);
                orderDetails = _mapper.Map<List<SessionServiceDto>>(bidaTableSession.ServiceSessions);
            }

            var bidaTableSessionPaymentDto = new BidaTableSessionPaymentDto
            {
                Code = bidaTableSession.BidaTable.Code,
                BidaTableTypeName = bidaTableSession.BidaTable.BidaTableType.Name,
                Price = price,
                TotalTime = totalTime,
                TotalTimeString = totalTimeString,
                StartTime = startTime,
                EndTime = endTime,
                DiscountType = DiscountType.Percent,
                TotalServicePrice = totalServicePrice,
                SessionServiceDtos = orderDetails,
                TotalPrice = totalPrice,
                ParentId = parentId
            };

            return bidaTableSessionPaymentDto;
        }

        public async Task<List<SessionServiceDto>> GetAllByTableId(string bidaTableId)
        {
            var bidaTableSession = await _repository.Find(match: x => x.BidaTableId == bidaTableId
                                                          && x.SessionStatus == SessionStatus.Playing,
                                                          include: inc => inc.Include(x => x.ServiceSessions),
                                                          orderBy: s => s.OrderByDescending(x => x.CreatedDate));

            List<SessionServiceDto> orderDetails = new List<SessionServiceDto>();

            if (bidaTableSession == null) return orderDetails;


            if (bidaTableSession.ServiceSessions.Any())
            {
                orderDetails = _mapper.Map<List<SessionServiceDto>>(bidaTableSession.ServiceSessions);
            }

            return orderDetails;
        }

        public async Task<bool> UpdateServiceSession(ServiceInSessionDto model, string email)
        {
            if (model == null || string.IsNullOrEmpty(email))
            {
                throw new AppException  ("Dữ liệu không hợp lệ", 400);
            }

            var bidaTableSession = await GetBidaTableSession(model.BidaTableId);
            if (bidaTableSession == null) return false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await UpdateServices(bidaTableSession, model, email);
                    await _unitOfWork.SaveChangeAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        private async Task<BidaTableSession> GetBidaTableSession(string bidaTableId)
        {
            return await _repository.Find(
                match: x => x.BidaTableId == bidaTableId && x.SessionStatus == SessionStatus.Playing,
                include: inc => inc.Include(x => x.ServiceSessions),
                orderBy: s => s.OrderByDescending(x => x.CreatedDate)
            );
        }

        private async Task UpdateServices(BidaTableSession bidaTableSession, ServiceInSessionDto model, string email)
        {
            var currentServices = bidaTableSession.ServiceSessions.ToList();
            var updatedServices = new List<ServiceSession>();

            if (model.SessionServiceCreateDtos == null || !model.SessionServiceCreateDtos.Any())
            {
                await RemoveAllServices(currentServices);
                bidaTableSession.ServiceSessions = new List<ServiceSession>();
            }
            else
            {
                foreach (var serviceDto in model.SessionServiceCreateDtos)
                {
                    await UpdateOrAddService(bidaTableSession, currentServices, updatedServices, serviceDto, email);
                }
                await RemoveUnusedServices(currentServices, updatedServices);
                bidaTableSession.ServiceSessions = updatedServices;
            }

            await _repository.Update(bidaTableSession);
        }

        private async Task RemoveAllServices(List<ServiceSession> currentServices)
        {
            foreach (var service in currentServices)
            {
                await UpdateInventory(service.ProductId, service.Quantity);
                await _serviceSessionRepository.Delete(service);
            }
        }

        private async Task UpdateOrAddService(BidaTableSession bidaTableSession, List<ServiceSession> currentServices, List<ServiceSession> updatedServices, SessionServiceCreateDto serviceDto, string email)
        {
            var existingService = currentServices.FirstOrDefault(s => s.ProductId == serviceDto.ProductId);
            var inventory = await _inventoryRepository.Find(x => x.ProductId == serviceDto.ProductId,
                                                               include: s => s.Include(x => x.Product));

            if (inventory == null || !IsInventoryQuantitySufficient(inventory, serviceDto, existingService))
            {
                throw new AppException($"Số lượng tồn kho của {inventory.Product.Name.ToUpper()} không đủ", 400);
            }

            if (existingService != null)
            {
                await UpdateExistingService(existingService, serviceDto, inventory, email);
                updatedServices.Add(existingService);
            }
            else
            {
                var newService = await CreateNewService(bidaTableSession, serviceDto, inventory, email);
                updatedServices.Add(newService);
            }

            await _inventoryRepository.Update(inventory);
        }

        private bool IsInventoryQuantitySufficient(Inventory inventory, SessionServiceCreateDto serviceDto, ServiceSession existingService)
        {
            var requiredQuantity = existingService == null ? serviceDto.Quantity : serviceDto.Quantity - existingService.Quantity;
            return inventory.Quantity >= requiredQuantity;
        }

        private async Task UpdateExistingService(ServiceSession existingService, SessionServiceCreateDto serviceDto, Inventory inventory, string email)
        {
            var quantityDifference = serviceDto.Quantity - existingService.Quantity;
            if (quantityDifference == 0) return; // Không thay đổi số lượng sản phẩm thì không cần update
            existingService.Quantity = serviceDto.Quantity;
            existingService.UpdatedBy = email;
            await _serviceSessionRepository.Update(existingService);
            inventory.Quantity -= quantityDifference;
        }

        private async Task<ServiceSession> CreateNewService(BidaTableSession bidaTableSession, SessionServiceCreateDto serviceDto, Inventory inventory, string email)
        {
            var newService = _mapper.Map<ServiceSession>(serviceDto);
            newService.Id = Guid.NewGuid().ToString();
            newService.BidaTableSessionId = bidaTableSession.Id;
            newService.CreatedBy = email;
            newService.UpdatedBy = email;
            await _serviceSessionRepository.Add(newService);
            inventory.Quantity -= serviceDto.Quantity;
            return newService;
        }

        private async Task RemoveUnusedServices(List<ServiceSession> currentServices, List<ServiceSession> updatedServices)
        {
            var servicesToRemove = currentServices.Except(updatedServices).ToList();
            foreach (var serviceToRemove in servicesToRemove)
            {
                await UpdateInventory(serviceToRemove.ProductId, serviceToRemove.Quantity);
                await _serviceSessionRepository.Delete(serviceToRemove);
            }
        }

        private async Task UpdateInventory(string productId, int quantity)
        {
            var inventory = await _inventoryRepository.Find(x => x.ProductId == productId);
            if (inventory != null)
            {
                inventory.Quantity += quantity;
                await _inventoryRepository.Update(inventory);
            }
        }



    }
}
