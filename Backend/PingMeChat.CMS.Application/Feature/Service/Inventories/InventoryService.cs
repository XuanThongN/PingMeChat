using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.Inventories.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.Inventorys
{
    public interface IInventoryService : IServiceBase<Inventory, InventoryCreateDto, InventoryUpdateDto, InventoryDto, IInventoryRepository>
    {
    
    }
    public class InventoryService : ServiceBase<Inventory, InventoryCreateDto, InventoryUpdateDto, InventoryDto, IInventoryRepository>, IInventoryService
    {
        public InventoryService(IInventoryRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IUriService uriService) : base(repository, unitOfWork, mapper, uriService)
        {
        }

       
    }
}
