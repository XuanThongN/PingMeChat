using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Users;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Module;

namespace PingMeChat.CMS.Application.Feature.Service.Messages
{
    public interface IMessageService : IServiceBase<Message, MessageCreateDto, MessageUpdateDto, MessageDto, IMessageRepository>
    {
        Task<MessageDto> SendMessageAsync(string chatId, string userId, string content);
    }

    public class MessageService : ServiceBase<Message, MessageCreateDto, MessageUpdateDto, MessageDto, IMessageRepository>, IMessageService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly ILogErrorRepository _logErrorRepository;

        public MessageService(
            IMessageRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IUserChatRepository userChatRepository,
            ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _userChatRepository = userChatRepository;
            _logErrorRepository = logErrorRepository;
        }

        public async Task<MessageDto> SendMessageAsync(string chatId, string userId, string content)
        {
            try
            {
                var userChat = await _userChatRepository.Find(uc => uc.ChatId == chatId && uc.UserId == userId);
                if (userChat == null)
                {
                    throw new AppException("Người dùng không thuộc đoạn chat này", 403);
                }

                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = userId,
                    Content = content,
                    SentAt = DateTime.UtcNow
                };

                await _repository.Add(message);
                await _unitOfWork.SaveChangeAsync();

                return _mapper.Map<MessageDto>(message);
            }
            catch (Exception ex)
            {
                await _logErrorRepository.Add(new ErrorLog
                {
                    ControllerName = "MessageService",
                    ActionName = "SendMessageAsync",
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
