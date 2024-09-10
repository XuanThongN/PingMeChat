using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys
{
    public interface IOrderHistoryService : IServiceBase<OrderHistory, OrderHistoryCreateDto, OrderHistoryDto, OrderHistoryDto, IOrderHistoryRepository>
    {
    }

    public class OrderHistoryService : ServiceBase<OrderHistory, OrderHistoryCreateDto, OrderHistoryDto, OrderHistoryDto, IOrderHistoryRepository>, IOrderHistoryService
    {
        public OrderHistoryService(IOrderHistoryRepository orderRepository, IUnitOfWork unitOfWork, IMapper mapper, IUriService uriService) : base(orderRepository, unitOfWork, mapper, uriService)
        {
        }

    }
}
