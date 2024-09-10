using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.InventoryExports.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PingMeChat.CMS.Application.Feature.Service.InventoryExports
{
    public interface IInventoryExportService : IServiceBase<InventoryExport, InventoryExportCreateDto, InventoryExportUpdateDto, InventoryExportDto, IInventoryExportRepository>
    {
        Task<bool> Add(InventoryExportCreateDto dto, string email);
    }
    public class InventoryExportService : ServiceBase<InventoryExport, InventoryExportCreateDto, InventoryExportUpdateDto, InventoryExportDto, IInventoryExportRepository>, IInventoryExportService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<InventoryExportService> _logger;

        public InventoryExportService(IInventoryExportRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository,
             ILogger<InventoryExportService> logger) : base(repository, unitOfWork, mapper, uriService)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<bool> Add(InventoryExportCreateDto dto, string email)
        {
            // Kiểm tra số lượng hàng tồn kho có đủ để xuất kho không
            await CheckInventoryAvailability(dto.InventoryDetailsExports);
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var entity = _mapper.Map<InventoryExport>(dto);
                    entity.CreatedBy = email;
                    entity.UpdatedBy = email;
                    entity.InventoryType = InventoryType.Waiting;
                    entity.Code = await GenerateUniqueCodeAsync();
                    await _repository.Add(entity);

                    // Cập nhật lượng tồn kho
                    await UpdateInventory(dto.InventoryDetailsExports, email);

                    await _unitOfWork.SaveChangeAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occurred while adding inventory import");
                    return false;
                }
            });
        }

        // cập nhật lượng sản phẩm tồn kho
        private async Task UpdateInventory(List<InventoryDetailsExport>? list, string email)
        {
            if (list == null || list.Count == 0)
            {
                _logger.LogWarning("No products found in the import list");
                return;

            }
            var productIds = list.Select(x => x.ProductId).Distinct().ToList();
            var inventories = await _inventoryRepository.FindAll(x => productIds.Contains(x.ProductId));

            var inventoryDictionary = inventories.ToDictionary(x => x.ProductId, x => x);

            var inventoriesToUpdate = new List<Inventory>();

            foreach (var item in list)
            {
                if (inventoryDictionary.TryGetValue(item.ProductId, out var inventory))
                {
                    inventory.Quantity -= item.Quantity;
                    inventory.UpdatedBy = email;
                    inventoriesToUpdate.Add(inventory);
                }
            }

            if (inventoriesToUpdate.Any())
            {
                await _inventoryRepository.UpdateRange(inventoriesToUpdate);
            }

        }

        // kiểm tra số lượng hàng tồn kho có đủ để xuất kho không
        private async Task CheckInventoryAvailability(List<InventoryDetailsExport>? list)
        {
            if (list == null || list.Count == 0)
            {
                throw new AppException("Không tìm thấy sản phẩm nào trong danh sách xuất kho");
            }

            var productIds = list.Select(x => x.ProductId).Distinct().ToList();
            var inventories = await _inventoryRepository.FindAll(x => productIds.Contains(x.ProductId));

            var inventoryDictionary = inventories.ToDictionary(x => x.ProductId, x => x);

            foreach (var item in list)
            {
                if (inventoryDictionary.TryGetValue(item.ProductId, out var inventory))
                {
                    if (inventory.Quantity < item.Quantity)
                    {
                        // Số lượng hàng tồn kho không đủ
                        throw new AppException($"Số lượng hàng tồn kho không đủ để xuất kho cho sản phẩm {item.ProductName}");
                    }
                }
                else
                {
                    // Không tìm thấy hàng tồn kho cho mặt hàng
                    throw new AppException($"Không tìm thấy hàng tồn kho cho sản phẩm {item.ProductName}");
                }
            }
        }



        #region tạo mã xuất kho
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

    }
}
