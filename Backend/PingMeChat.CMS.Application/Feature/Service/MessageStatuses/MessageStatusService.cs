using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.MessageStatuses.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Module;

namespace PingMeChat.CMS.Application.Feature.Service.MessageStatuses
{
    public interface IMessageStatusService : IServiceBase<MessageStatus, MessageStatusCreateDto, MessageStatusUpdateDto, MessageStatusDto, IMessageStatusRepository>
    {
        Task<bool> MarkAsReadAsync(string messageId, string userId);
    }
    public class MessageStatusService : ServiceBase<MessageStatus, MessageStatusCreateDto, MessageStatusUpdateDto, MessageStatusDto, IMessageStatusRepository>, IMessageStatusService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        public MessageStatusService(IMessageStatusRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IUserChatRepository userChatRepository,
            IMessageRepository messageRepository,
             ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _userChatRepository = userChatRepository;
            _messageRepository = messageRepository;
            _logErrorRepository = logErrorRepository;
        }

        public async Task<bool> MarkAsReadAsync(string messageId, string userId)
        {
            try
            {
                var message = await _messageRepository.Find(
                                            match: ms => ms.Id == messageId,
                                            include: query => query
                                                .Include(m => m.Chat)
                                                .ThenInclude(c => c.UserChats),
                                            asNoTracking: true
                                        );
                if (message == null || !message.Chat.UserChats.Any(uc => uc.UserId == userId))
                {
                    throw new AppException("User is not a member of this chat", 403);
                }

                var existingStatus = await _repository.Find(ms => ms.MessageId == messageId && ms.UserId == userId);
                if (existingStatus == null)
                {
                    var messageStatus = new MessageStatus
                    {
                        MessageId = messageId,
                        UserId = userId,
                        ReadAt = DateTime.UtcNow
                    };

                    await _repository.Add(messageStatus);
                    await _unitOfWork.SaveChangeAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                await _logErrorRepository.Add(new ErrorLog
                {
                    ControllerName = "MessageStatusService",
                    ActionName = "MarkChatAsReadAsync",
                    IsError = true,
                    ErrorMessage = ex.Message,
                    Exception = ex.StackTrace
                });
                await _unitOfWork.SaveChangeAsync();
                throw;
            }
        }
    }
}
