using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.SessionServices.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.SessionServices
{
    public interface IServiceSessionService : IServiceBase<ServiceSession, SessionServiceCreateDto, SessionServiceUpdateDto, SessionServiceDto, IServiceSessionRepository>
    {
    }
    public class ServiceSessionService : ServiceBase<ServiceSession, SessionServiceCreateDto, SessionServiceUpdateDto, SessionServiceDto, IServiceSessionRepository>, IServiceSessionService
    {
        public ServiceSessionService(IServiceSessionRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IUriService uriService) : base(repository, unitOfWork, mapper, uriService)
        {
        }
    }
}
