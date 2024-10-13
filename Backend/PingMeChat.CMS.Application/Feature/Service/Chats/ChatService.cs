using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Application.Feature.Service.UserChats.Dto;
using PingMeChat.CMS.Application.Feature.Services.RedisCacheServices;
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
        Task<List<string>> GetChatIdListAsync(string userId);
        Task<PagedResponse<List<ChatDto>>> GetChatListAsync(string chatId, int pageNumber, int pageSize, string route = null);
        Task<List<UserChatDto>> AddUsersToChatAsync(string chatId, List<string> userIds, string currentUserId);
        Task<bool> RemoveUserFromChatAsync(string chatId, string userId, string currentUser);
        Task<bool> CanUserAccessChat(string chatId, string userId);
    }

    public class ChatService : ServiceBase<Chat, ChatCreateDto, ChatUpdateDto, ChatDto, IChatRepository>, IChatService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly IAccountRepository _userRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMemoryCache _cache; // Bộ nhớ cache để lưu trữ số lượng lệnh trong khoảng thời gian
        private readonly ICacheService _cacheService; // cache redis
        private const string ChatListCacheKey = "ChatList_{0}_{1}_{2}"; // userId_page_pageSize
        private const string ChatDetailCacheKey = "ChatDetail_{0}"; // chatId
        private const string ChatIdList = "ChatIdList_{0}"; // Danh sách Id chat của user

        public ChatService(
            IChatRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IUserChatRepository userChatRepository,
            ILogErrorRepository logErrorRepository,
            IMessageRepository messageRepository,
            IMemoryCache cache,
            ICacheService cacheService,
            IAccountRepository userRepository
            ) : base(repository, unitOfWork, mapper, uriService)
        {
            _userChatRepository = userChatRepository;
            _logErrorRepository = logErrorRepository;
            _messageRepository = messageRepository;
            _cache = cache;
            _cacheService = cacheService;
            _userRepository = userRepository;
        }

        public async Task<ChatDto> CreateChatAsync(ChatCreateDto chatCreateDto, string userId)
        {
            try
            {
                // Kiểm tra nếu chat riêng tư đã tồn tại
                if (!chatCreateDto.IsGroup)
                {
                    var existingChat = await CheckIfPrivateChatExists(chatCreateDto, userId);
                    if (existingChat != null)
                    {
                        return existingChat;
                    }
                }

                var chat = _mapper.Map<Chat>(chatCreateDto);
                chat.CreatedBy = userId;
                chat.UpdatedBy = userId;

                await _repository.Add(chat);
                await _unitOfWork.SaveChangeAsync();

                // Thêm người dùng vào đoạn chat
                var allUserIds = chatCreateDto.UserIds.Append(userId);
                await AddUsersToChat(allUserIds, chat.Id, userId, chatCreateDto.IsGroup);

                // Load đoạn chat với UserChats và Users
                var createdChat = await _repository.Find(c => c.Id == chat.Id,
                    include: o => o.Include(e => e.UserChats).ThenInclude(uc => uc.User)
                );
                var result = _mapper.Map<ChatDto>(createdChat);
                await AddChatToCache(result);

                return result;
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex, "CreateChatAsync");
                throw new AppException("Error occurred while creating chat.", 500);
            }
        }

        private async Task<ChatDto> CheckIfPrivateChatExists(ChatCreateDto chatCreateDto, string userId)
        {
            var existingChat = await _repository.Find(c =>
                !c.IsGroup &&
                c.UserChats.Any(uc => uc.UserId == userId) &&
                c.UserChats.Any(uc => uc.UserId == chatCreateDto.UserIds.First()),
                include: o => o.Include(e => e.UserChats).ThenInclude(uc => uc.User)
            );

            return existingChat != null ? _mapper.Map<ChatDto>(existingChat) : null;
        }

        private async Task AddUsersToChat(IEnumerable<string> userIds, string chatId, string createdBy, bool isGroup)
        {
            foreach (var userId in userIds)
            {
                var userChat = new UserChat
                {
                    UserId = userId,
                    ChatId = chatId,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    IsAdmin = isGroup ? userId == createdBy : false,
                    JoinAt = DateTime.UtcNow,
                };
                await _userChatRepository.Add(userChat);
            }
            await _unitOfWork.SaveChangeAsync();
        }

        private async Task LogErrorAsync(Exception ex, string actionName)
        {
            await _logErrorRepository.Add(new ErrorLog
            {
                ControllerName = "ChatService",
                ActionName = actionName,
                IsError = true,
                ErrorMessage = ex.Message,
                Exception = ex.StackTrace
            });
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<ChatDto> GetChatDetailAsync(string chatId, string userId)
        {
            try
            {
                string cacheKey = string.Format(ChatDetailCacheKey, chatId);
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var chat = await _repository.Find(
                        c => c.Id == chatId,
                        include: q => q
                        .Include(c => c.UserChats).ThenInclude(uc => uc.User) // Bao gồm thông tin người dùng
                        .Include(c => c.Messages.OrderByDescending(m => m.CreatedDate).Take(1))
                        .ThenInclude(m => m.Sender)
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
                },
                TimeSpan.FromMinutes(5) // Cache for 5 minutes 
                );
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


        public async Task<List<string>> GetChatIdListAsync(string userId)
        {
            try
            {
                string cacheKey = string.Format(ChatIdList, userId);
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var chat = await _repository.FindAll(
                       c => c.UserChats.Any(x => x.UserId == userId)
                    ) ?? throw new AppException("Chat not found", 404);
                    var chatIds = chat.Select(c => c.Id).ToList();
                    return chatIds;
                },
                    TimeSpan.FromHours(1) // Cache for 5 minutes 
                );
            }
            catch (Exception ex)
            {
                throw new AppException("Error occurred while getting chat list", 500);
            }
        }
        public async Task<PagedResponse<List<ChatDto>>> GetChatListAsync(
            string userId,
            int pageNumber,
            int pageSize,
            string route = null)
        {
            string cacheKey = string.Format(ChatListCacheKey, userId, pageNumber, pageSize);
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
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
            },
            TimeSpan.FromMinutes(5) // Cache for 5 minutes
            );
        }

        public async Task<List<UserChatDto>> AddUsersToChatAsync(string chatId, List<string> userIds, string currentUserId)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var chat = await _repository.Find(c => c.Id == chatId, include: q => q.Include(c => c.UserChats));
                        if (chat == null)
                        {
                            throw new AppException("Chat not found", 404);
                        }

                        // Kiểm tra xem đây có phải là chat nhóm không
                        if (!chat.IsGroup)
                        {
                            throw new AppException("Không thể thêm người dùng vào chat cá nhân", 400);
                        }

                        var existingUserChats = await _userChatRepository.FindAll(uc => uc.ChatId == chatId && userIds.Contains(uc.UserId));
                        var existingUserIds = existingUserChats.Select(uc => uc.UserId).ToHashSet();

                        var newUserChats = userIds
                            .Where(userId => !existingUserIds.Contains(userId))
                            .Select(userId => new UserChat
                            {
                                UserId = userId,
                                ChatId = chatId,
                                CreatedBy = currentUserId,
                                UpdatedBy = currentUserId,
                                IsAdmin = false,
                                JoinAt = DateTime.UtcNow
                            }).ToList();

                        if (newUserChats.Count == 0)
                        {
                            throw new AppException("Tất cả người dùng đã là thành viên của chat này", 400);
                        }

                        await _userChatRepository.AddRange(newUserChats);
                        await _unitOfWork.SaveChangeAsync();

                        await transaction.CommitAsync();
                        var result = await _userChatRepository.FindAll(uc => uc.ChatId == chatId && newUserChats.Select(nuc => nuc.UserId).Contains(uc.UserId), include: q => q.Include(uc => uc.User));
                        return _mapper.Map<List<UserChatDto>>(result);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        await _logErrorRepository.Add(new ErrorLog
                        {
                            ControllerName = "ChatService",
                            ActionName = "AddUsersToChatAsync",
                            IsError = true,
                            ErrorMessage = ex.Message,
                            Exception = ex.StackTrace
                        });
                        await _unitOfWork.SaveChangeAsync();
                        throw;
                    }
                }
            });
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

        // Hàm thêm đoạn chat vừa tạo vào cache
        private async Task AddChatToCache(ChatDto chatDto)
        {
            int page = 1;
            int pageSize = 20;
            PagedResponse<List<ChatDto>> cachedChatList = null;
            List<ChatDto> cachedChatListDat = null;

            // Tạo tin nhắn thông báo chat mới (hard code)
            var creator = await _userRepository.FindById(chatDto.CreatedBy);
            var messageContent = $"{creator.FullName} added you to a new chat";
            chatDto.Messages = new List<MessageDto> {
                new MessageDto {
                    ChatId = chatDto.Id,
                    Content = messageContent,
                    Sender = _mapper.Map<AccountDto>(creator),
                    SenderId = creator.Id,
                    CreatedDate = DateTime.UtcNow
                }
            };
            // Tìm list user tham gia chat
            var userChats = chatDto.UserChats;
            var userIds = userChats.Select(uc => uc.UserId).ToList();
            foreach (var userId in userIds)
            {
                // Thêm chat vào cache của từng user
                var cacheKey = string.Format(ChatListCacheKey, userId, page, pageSize);
                cachedChatList = await _cacheService.GetAsync<PagedResponse<List<ChatDto>>>(cacheKey);
                if (cachedChatList != null)
                {
                    cachedChatListDat = cachedChatList.Data;
                    // Thêm chat mới vào đầu danh sách, trước khi lưu vào cache thì thêm tin nhắn mới nhất vào chat
                    chatDto.Messages = new List<MessageDto>
                    {
                        // Tạo tin nhắn thông báo chat mới
                        new MessageDto
                        {
                            ChatId = chatDto.Id,
                            Content = "Created a new chat",
                            Sender = chatDto.Messages.FirstOrDefault()?.Sender,
                            SenderId = chatDto.Messages.FirstOrDefault()?.SenderId,
                            CreatedDate = DateTime.UtcNow
                        }
                    };
                    cachedChatListDat.Insert(0, chatDto);
                    // Lưu danh sách chat mới vào cache
                    await _cacheService.SetAsync(cacheKey, cachedChatListDat, TimeSpan.FromMinutes(5));
                }

            }

        }
    }
}


