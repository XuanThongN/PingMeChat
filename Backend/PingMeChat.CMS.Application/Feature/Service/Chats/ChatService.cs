using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
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
        Task<PagedResponse<List<ChatDto>>> GetChatListAsync(string chatId, int pageNumber, int pageSize, string route = null);
        Task<ChatDto> AddUserToChatAsync(string chatId, string userId, string currentUserId);
        Task<bool> RemoveUserFromChatAsync(string chatId, string userId, string currentUser);
        Task<bool> CanUserAccessChat(string chatId, string userId);
    }

    public class ChatService : ServiceBase<Chat, ChatCreateDto, ChatUpdateDto, ChatDto, IChatRepository>, IChatService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMemoryCache _cache; // Bộ nhớ cache để lưu trữ số lượng lệnh trong khoảng thời gian

        public ChatService(
            IChatRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IUserChatRepository userChatRepository,
            ILogErrorRepository logErrorRepository,
            IMessageRepository messageRepository,
            IMemoryCache cache) : base(repository, unitOfWork, mapper, uriService)
        {
            _userChatRepository = userChatRepository;
            _logErrorRepository = logErrorRepository;
            _messageRepository = messageRepository;
            _cache = cache;
        }

        public async Task<ChatDto> CreateChatAsync(ChatCreateDto chatCreateDto, string userId)
        {
            try
            {
                // Check if private chat already exists
                if (!chatCreateDto.IsGroup)
                {
                    var existingChat = await _repository.Find(c =>
                        !c.IsGroup &&
                        c.UserChats.Any(uc => uc.UserId == userId) &&
                        c.UserChats.Any(uc => uc.UserId == chatCreateDto.UserIds.First()),
                        include: o => o.Include(e => e.UserChats).ThenInclude(uc => uc.User)
                    );

                    if (existingChat != null)
                    {
                        return _mapper.Map<ChatDto>(existingChat);
                    }
                }

                var chat = _mapper.Map<Chat>(chatCreateDto);
                chat.CreatedBy = userId;
                chat.UpdatedBy = userId;

                await _repository.Add(chat);
                await _unitOfWork.SaveChangeAsync();

                // Add chat creator to the member list
                var allUserIds = chatCreateDto.UserIds.Append(userId);

                foreach (var memberId in allUserIds)
                {
                    var userChat = new UserChat
                    {
                        UserId = memberId,
                        ChatId = chat.Id,
                        CreatedBy = userId,
                        UpdatedBy = userId,
                        IsAdmin = chatCreateDto.IsGroup ? memberId == userId : false, // Set người tạo chat là admin nếu chat là group, còn chat private thì không có admin
                        JoinAt = DateTime.UtcNow,
                    };
                    await _userChatRepository.Add(userChat);
                }
                await _unitOfWork.SaveChangeAsync();

                // Load the chat with UserChats and Users included
                var createdChat = await _repository.Find(c => c.Id == chat.Id,
                    include: o => o.Include(e => e.UserChats).ThenInclude(uc => uc.User)
                );

                return _mapper.Map<ChatDto>(createdChat);
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
                var chats = await _repository.FindAll(c => c.UserChats.Any(x => x.UserId == userId));
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
        public async Task<PagedResponse<List<ChatDto>>> GetChatListAsync(
    string userId,
    int pageNumber,
    int pageSize,
    string route = null)
        {
            return await Pagination(
                pageNumber,
                pageSize,
                predicate: c => c.UserChats.Any(uc => uc.UserId == userId) && (c.IsGroup || c.Messages.Any()),
                orderBy: q => q.OrderByDescending(c => c.Messages
                    .OrderByDescending(m => m.CreatedDate)
                    .Select(m => (DateTime?)m.CreatedDate)
                    .FirstOrDefault() ?? c.CreatedDate), // Nếu không có tin nhắn, dùng DateTime.MinValue để xếp cuối cùng
                include: q => q
                    .Include(c => c.UserChats).ThenInclude(uc => uc.User) // Bao gồm thông tin người dùng
                    .Include(c => c.Messages.OrderByDescending(m => m.CreatedDate).Take(1))
                    .ThenInclude(m => m.Sender)
                    , // Lấy tin nhắn cuối cùng
                route: route
            );
        }



        public async Task<ChatDto> AddUserToChatAsync(string chatId, string userId, string currentUserId)
        {
            try
            {
                var chat = await _repository.FindById(chatId);
                if (chat == null)
                {
                    throw new AppException("Chat not found", 404);
                }

                // Kiểm tra xem đây có phải là chat nhóm không
                if (!chat.IsGroup)
                {
                    throw new AppException("Không thể thêm người dùng vào chat cá nhân", 400);
                }

                var existingUserChat = await _userChatRepository.Find(uc => uc.ChatId == chatId && uc.UserId == userId);
                if (existingUserChat != null)
                {
                    throw new AppException("Người dùng đã là thành viên của chat này", 400);
                }

                var userChat = new UserChat
                {
                    UserId = userId,
                    ChatId = chatId,
                    CreatedBy = currentUserId,
                    UpdatedBy = currentUserId,
                    IsAdmin = false,
                    JoinAt = DateTime.UtcNow
                };
                await _userChatRepository.Add(userChat);
                await _unitOfWork.SaveChangeAsync();

                // Refresh chat data
                chat = await _repository.Find(
                    c => c.Id == chatId,
                    include: c => c
                        .Include(c => c.UserChats)
                        .ThenInclude(uc => uc.User)
                );

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

        // Hàm kiểm tra xem người dùng có thể truy cập vào chat hay không
        // chatId: ID của chat
        // userId: ID của người dùng
        // Trả về true nếu người dùng có thể truy cập vào chat, ngược lại trả về false
        public async Task<bool> CanUserAccessChat(string chatId, string userId)
        {
            // Tạo một key cache dựa trên chatId và userId
            var cacheKey = $"ChatAccess_{chatId}_{userId}";

            // Kiểm tra xem kết quả đã được lưu trong cache hay chưa
            if (!_cache.TryGetValue(cacheKey, out bool canAccess))
            {
                // Nếu không có kết quả trong cache, thì thực hiện truy vấn cơ sở dữ liệu
                canAccess = await _repository.AnyAsync(c => c.Id == chatId && c.UserChats.Any(uc => uc.UserId == userId));

                // Lưu kết quả vào cache với thời gian sống là 5 phút
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, canAccess, cacheEntryOptions);
            }

            return canAccess;
        }
    }

}
