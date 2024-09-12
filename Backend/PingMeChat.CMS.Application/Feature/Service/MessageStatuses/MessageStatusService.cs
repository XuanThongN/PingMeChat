using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.MessageStatuses.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.MessageStatuses
{
    public interface IMessageStatusService : IServiceBase<MessageStatus, MessageStatusCreateDto, MessageStatusUpdateDto, MessageStatusDto, IMessageStatusRepository>
    {
    }
    public class MessageStatusService : ServiceBase<MessageStatus, MessageStatusCreateDto, MessageStatusUpdateDto, MessageStatusDto, IMessageStatusRepository>, IMessageStatusService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        public MessageStatusService(IMessageStatusRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IUserChatRepository userChatRepository,
             ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _userChatRepository = userChatRepository;
            _logErrorRepository = logErrorRepository;
        }
    }
}
