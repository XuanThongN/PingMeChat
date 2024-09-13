using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Module;
using System.Linq.Expressions;

namespace PingMeChat.CMS.Application.Feature.Service.Chats
{
    public interface IChatService : IServiceBase<Chat, ChatCreateDto, ChatUpdateDto, ChatDto, IChatRepository>
    {
        Task<ChatDto> CreateChatAsync(ChatCreateDto chatCreateDto, string userId);
        Task<ChatDto> GetChatDetailAsync(string chatId, string userId);
        Task<IEnumerable<ChatDto>> GetChatListAsync(string userId);
        Task<ChatDto> AddUserToChatAsync(string chatId, string userId);
        Task<bool> RemoveUserFromChatAsync(string chatId, string userId, string currentUser);
    }

    public class ChatService : ServiceBase<Chat, ChatCreateDto, ChatUpdateDto, ChatDto, IChatRepository>, IChatService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly IMessageRepository _messageRepository;

        public ChatService(
            IChatRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IUserChatRepository userChatRepository,
            ILogErrorRepository logErrorRepository,
            IMessageRepository messageRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _userChatRepository = userChatRepository;
            _logErrorRepository = logErrorRepository;
            _messageRepository = messageRepository;
        }

        public async Task<ChatDto> CreateChatAsync(ChatCreateDto chatCreateDto, string userId)
        {
            try
            {
                var chat = _mapper.Map<Chat>(chatCreateDto);
                chat.CreatedBy = userId;
                chat.UpdatedBy = userId;

                await _repository.Add(chat);
                await _unitOfWork.SaveChangeAsync();

                // Thêm người tạo chat vào danh sách thành viên
                chatCreateDto.UserChatIds = chatCreateDto.UserChatIds.Append(userId);
                // Thêm các thành viên còn lại vào danh sách thành viên
                foreach (var userChatId in chatCreateDto.UserChatIds)
                {
                    var userChat = new UserChat { 
                        UserId = userChatId, 
                        ChatId = chat.Id,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        IsAdmin = userChatId == userId,
                        JoinAt = DateTime.UtcNow,

                    };
                    await _userChatRepository.Add(userChat);
                }
                await _unitOfWork.SaveChangeAsync();

                return _mapper.Map<ChatDto>(chat);
            }
            catch (Exception ex)
            {
                await _logErrorRepository.Add(new ErrorLog
                {
                    ControllerName = "ChatService",
                    ActionName = "CreateChatAsync",
                    IsError = true,
                    ErrorMessage = ex.Message,
                    Exception = ex.StackTrace
                });
                await _unitOfWork.SaveChangeAsync();
                throw new AppException("Error occurred while creating chat.", 500);
            }
        }

        public async Task<ChatDto> GetChatDetailAsync(string chatId, string userId)
        {
            try
            {
                var chat = await _repository.Find(
                    c => c.Id == chatId,
                    include: c => c
                        .Include(c => c.UserChats)
                        .ThenInclude(uc => uc.User)
                        .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(20))
                );

                if (chat == null)
                {
                    throw new AppException("Chat not found", 404);
                }

                var userChat = chat.UserChats.FirstOrDefault(uc => uc.UserId == userId);
                if (userChat == null)
                {
                    throw new AppException("User is not a member of this chat", 403);
                }

                var chatDto = _mapper.Map<ChatDto>(chat);

                return chatDto;
            }
            catch (Exception ex)
            {
                await _logErrorRepository.Add(new ErrorLog
                {
                    ControllerName = "ChatService",
                    ActionName = "GetChatDetailAsync",
                    IsError = true,
                    ErrorMessage = ex.Message,
                    Exception = ex.StackTrace
                });
                await _unitOfWork.SaveChangeAsync();
                throw;
            }
        }


        public async Task<IEnumerable<ChatDto>> GetChatListAsync(string userId)
        {
            try
            {
                var userChats = await _userChatRepository.FindAll(uc => uc.UserId == userId);
                var chatIds = userChats.Select(uc => uc.ChatId);
                var chats = await _repository.FindAll(c => chatIds.Contains(c.Id));
                return _mapper.Map<IEnumerable<ChatDto>>(chats);
            }
            catch (Exception ex)
            {
                await _logErrorRepository.Add(new ErrorLog
                {
                    ControllerName = "ChatService",
                    ActionName = "GetChatListAsync",
                    IsError = true,
                    ErrorMessage = ex.Message,
                    Exception = ex.StackTrace
                });
                await _unitOfWork.SaveChangeAsync();
                throw new AppException("Error occurred while retrieving chat list.", 500);
            }
        }

        public async Task<ChatDto> AddUserToChatAsync(string chatId, string userId)
        {
            try
            {
                var chat = await _repository.FindById(chatId);
                if (chat == null)
                {
                    throw new AppException("Chat not found", 404);
                }

                var existingUserChat = await _userChatRepository.Find(uc => uc.ChatId == chatId && uc.UserId == userId);
                if (existingUserChat != null)
                {
                    throw new AppException("User is already a member of this chat", 400);
                }

                var userChat = new UserChat { UserId = userId, ChatId = chatId };
                await _userChatRepository.Add(userChat);
                await _unitOfWork.SaveChangeAsync();

                return _mapper.Map<ChatDto>(chat);
            }
            catch (Exception ex)
            {
                await _logErrorRepository.Add(new ErrorLog
                {
                    ControllerName = "ChatService",
                    ActionName = "AddUserToChatAsync",
                    IsError = true,
                    ErrorMessage = ex.Message,
                    Exception = ex.StackTrace
                });
                await _unitOfWork.SaveChangeAsync();
                throw;
            }
        }

        public async Task<bool> RemoveUserFromChatAsync(string chatId, string userId, string currentUser)
        {
            try
            {
                // Kiểm tra xem người dùng hiện tại có quyền xoá thành viên khỏi chat không
                var currentUserChat = await _userChatRepository.Find(uc => uc.ChatId == chatId && uc.UserId == currentUser);
                if (currentUserChat == null || !currentUserChat.IsAdmin)
                {
                    throw new AppException("Bạn không có quyền xoá thành viên", 404);
                }
                // Kiểm tra xem người dùng cần xoá có phải là admin không
                if (currentUser == userId)
                {
                    throw new AppException("Không thể xoá chính mình", 403);
                }
                var userChat = await _userChatRepository.Find(uc => uc.ChatId == chatId && uc.UserId == userId);
                if (userChat == null)
                {
                    throw new AppException("User is not a member of this chat", 404);
                }
                await _userChatRepository.SoftDelete(userChat);
                await _unitOfWork.SaveChangeAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _logErrorRepository.Add(new ErrorLog
                {
                    ControllerName = "ChatService",
                    ActionName = "RemoveUserFromChatAsync",
                    IsError = true,
                    ErrorMessage = ex.Message,
                    Exception = ex.StackTrace
                });
                await _unitOfWork.SaveChangeAsync();
                throw;
            }
        }

        public async Task<MessageDto> SendMessageAsync(string chatId, string userId, string content)
        {
            try
            {
                var userChat = await _userChatRepository.Find(uc => uc.ChatId == chatId && uc.UserId == userId);
                if (userChat == null)
                {
                    throw new AppException("User is not a member of this chat", 403);
                }

                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = userId,
                    Content = content,
                    SentAt = DateTime.UtcNow
                };

                await _messageRepository.Add(message);
                await _unitOfWork.SaveChangeAsync();

                return _mapper.Map<MessageDto>(message);
            }
            catch (Exception ex)
            {
                await _logErrorRepository.Add(new ErrorLog
                {
                    ControllerName = "ChatService",
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
