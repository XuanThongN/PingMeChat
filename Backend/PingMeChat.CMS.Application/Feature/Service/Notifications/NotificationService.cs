using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.Notifications.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.Notifications
{
    public interface INotificationService : IServiceBase<Notification, NotificationCreateDto, NotificationUpdateDto, NotificationDto, INotificationRepository>
    {
    }
    public class NotificationService : ServiceBase<Notification, NotificationCreateDto, NotificationUpdateDto, NotificationDto, INotificationRepository>, INotificationService
    {
        private readonly ILogErrorRepository _logErrorRepository;
        public NotificationService(
            INotificationRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
             ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _logErrorRepository = logErrorRepository;
        }

    }
}
