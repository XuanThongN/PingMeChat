using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.InventoryImports.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PingMeChat.CMS.Application.Feature.Service.InventoryImports
{
    public interface IInventoryImportService : IServiceBase<InventoryImport, InventoryImportCreateDto, InventoryImportUpdateDto, InventoryImportDto, IInventoryImportRepository>
    {
        Task<bool> Add(InventoryImportCreateDto dto, string email);
    }
    public class InventoryImportService : ServiceBase<InventoryImport, InventoryImportCreateDto, InventoryImportUpdateDto, InventoryImportDto, IInventoryImportRepository>, IInventoryImportService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<InventoryImportService> _logger;

        public InventoryImportService(IInventoryImportRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository,
             ILogger<InventoryImportService> logger) : base(repository, unitOfWork, mapper, uriService)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<bool> Add(InventoryImportCreateDto dto, string email)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var entity = _mapper.Map<InventoryImport>(dto);
                    entity.CreatedBy = email;
                    entity.UpdatedBy = email;
                    entity.InventoryType = InventoryType.Waiting;
                    entity.Code = await GenerateUniqueCodeAsync();
                    await _repository.Add(entity);

                    // Cập nhật lượng tồn kho
                    await UpdateInventory(dto.InventoryDetailsImports, email);

                    // Cập nhật giá nhập và người cập nhật cho sản phẩm
                    await UpdateProductCostAndUpdater(dto.InventoryDetailsImports, email);

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
        private async Task UpdateInventory(List<InventoryDetailsImport>? list, string email)
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
            var newInventories = new List<Inventory>();

            foreach (var item in list)
            {
                if (inventoryDictionary.TryGetValue(item.ProductId, out var inventory))
                {
                    inventory.Quantity += item.Quantity;
                    inventory.UpdatedBy = email;
                    inventoriesToUpdate.Add(inventory);
                }
                else
                {
                    newInventories.Add(new Inventory
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        CreatedBy = email,
                        UpdatedBy = email
                    });
                }
            }

            if (inventoriesToUpdate.Any())
            {
                await _inventoryRepository.UpdateRange(inventoriesToUpdate);
            }

            if (newInventories.Any())
            {
                await _inventoryRepository.AddRange(newInventories);
            }

        }


        // câp nhật giá nhập và người điều chỉnh cho sản phẩm
        private async Task UpdateProductCostAndUpdater(List<InventoryDetailsImport> details, string email)
        {
            if (details == null || !details.Any())
            {
                _logger.LogWarning("No products to update cost and updater");
                return;
            }

            var productIds = details.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productRepository.FindAll(x => productIds.Contains(x.Id));

            var productsToUpdate = new List<Product>();

            foreach (var detail in details)
            {
                var product = products.FirstOrDefault(p => p.Id == detail.ProductId);
                if (product != null)
                {
                    product.Cost = detail.Cost;
                    product.UpdatedBy = email;
                    productsToUpdate.Add(product);
                }
                else
                {
                    _logger.LogWarning($"Product with ID {detail.ProductId} not found");
                }
            }

            if (productsToUpdate.Any())
            {
                await _productRepository.UpdateRange(productsToUpdate);
            }
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

    }
}
