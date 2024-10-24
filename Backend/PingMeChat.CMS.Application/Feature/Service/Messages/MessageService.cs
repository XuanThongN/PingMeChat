using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Module;
using PingMeChat.CMS.Application.Common.Pagination;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using PingMeChat.CMS.Application.Feature.ChatHubs;
using PingMeChat.CMS.Application.Feature.Service.Notifications;
using PingMeChat.CMS.Application.Feature.Services.RedisCacheServices;
using Org.BouncyCastle.Asn1.Cms;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using Microsoft.EntityFrameworkCore.Storage;

namespace PingMeChat.CMS.Application.Feature.Service.Messages
{
    public interface IMessageService : IServiceBase<Message, MessageCreateDto, MessageUpdateDto, MessageDto, IMessageRepository>
    {
        Task<MessageDto> SendMessageAsync(MessageCreateDto messageCreateDto);
        Task<PagedResponse<List<MessageDto>>> GetChatMessagesAsync(string chatId, int pageNumber, int pageSize, string route = null);
        // Mark message as read
        Task<bool> MarkMessageAsReadAsync(string messageId, string userId, string chatId);
        Task<bool> HasMessageAccess(string messageId, string userId);
    }

    public class MessageService : ServiceBase<Message, MessageCreateDto, MessageUpdateDto, MessageDto, IMessageRepository>, IMessageService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly IChatHubService _chatHubService;
        private readonly IAccountRepository _accountRepository;
        private readonly IFCMService _fcmService;
        private readonly ICacheService _cacheService; // cache redis
        private const string ChatMessagesCacheKey = "ChatMessages_{0}_{1}_{2}"; // chatId_page_pageSize
        private const string ChatListCacheKey = "ChatList_{0}_{1}_{2}"; // userId_page_pageSize

        public MessageService(
            IMessageRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IUserChatRepository userChatRepository,
            ILogErrorRepository logErrorRepository,
            IChatHubService chatHubService,
            IAccountRepository accountRepository,
            IFCMService fcmService,
            ICacheService cacheService)
            : base(repository, unitOfWork, mapper, uriService)
        {
            _userChatRepository = userChatRepository;
            _logErrorRepository = logErrorRepository;
            _chatHubService = chatHubService;
            _accountRepository = accountRepository;
            _fcmService = fcmService;
            _cacheService = cacheService; // cache redis
        }
        public override async Task<PagedResponse<List<MessageDto>>> Pagination(
        int pageNumber,
        int pageSize,
        Expression<Func<Message, bool>>? predicate = null,
        Expression<Func<Message, Message>>? selector = null,
        Func<IQueryable<Message>, IIncludableQueryable<Message, object>>? include = null,
        Func<IQueryable<Message>, IOrderedQueryable<Message>>? orderBy = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false,
        string route = null)
        {
            // Chắc chắn rằng chúng ta luôn bao gồm thông tin người gửi
            include = query => query.Include(m => m.Sender);

            // Nếu không có sắp xếp nào được chỉ định, sắp xếp theo thời gian gửi giảm dần
            orderBy = orderBy ?? (q => q.OrderByDescending(m => m.SentAt));

            return await base.Pagination(pageNumber, pageSize, predicate, selector, include, orderBy, disableTracking, ignoreQueryFilters, route);
        }

        public async Task<PagedResponse<List<MessageDto>>> GetChatMessagesAsync(
        string chatId,
        int pageNumber,
        int pageSize,
        string route = null)
        {
            string cacheKey = string.Format(ChatMessagesCacheKey, chatId, pageNumber, pageSize);
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                return await Pagination(
                    pageNumber,
                    pageSize,
                    predicate: m => m.ChatId == chatId,
                    orderBy: q => q.OrderByDescending(m => m.SentAt),
                    include: m => m.Include(o => o.Sender),
                    route: route
                    );
            }, TimeSpan.FromMinutes(2));
        }
        public async Task<MessageDto> SendMessageAsync(MessageCreateDto messageCreateDto)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var userChat = await GetUserChatAsync(messageCreateDto.ChatId, messageCreateDto.SenderId);
                        if (userChat == null)
                        {
                            throw new AppException("Người dùng không thuộc đoạn chat này", 403);
                        }

                        var attachments = CreateAttachments(messageCreateDto.Attachments);
                        var message = new Message
                        {
                            ChatId = messageCreateDto.ChatId,
                            SenderId = messageCreateDto.SenderId,
                            Content = messageCreateDto.Content,
                            Attachments = attachments,
                            SentAt = DateTime.UtcNow
                        };

                        await _repository.Add(message);
                        await _unitOfWork.SaveChangeAsync();

                        await transaction.CommitAsync();

                        // Lấy thông tin người gửi
                        message.Sender = userChat.User;

                        var result = _mapper.Map<MessageDto>(message);

                        // Chuyển việc cập nhật cache ra ngoài transaction
                        // _ = Task.WhenAll(
                        await UpdateMessagesInCache(result);
                        await UpdateChatListInCache(messageCreateDto.ChatId, result);
                        // ).ContinueWith(t =>
                        // {
                        //     if (t.IsFaulted)
                        //     {
                        //         // Log lỗi khi cập nhật cache thất bại
                        //         Console.WriteLine("Error updating cache: " + t.Exception.Message);
                        //     }
                        // });



                        return result;
                    }
                    catch (Exception ex)
                    {
                        // Chỉ rollback nếu transaction chưa commit
                        if (transaction.GetDbTransaction()?.Connection != null)
                        {
                            await transaction.RollbackAsync();
                        }
                        await LogErrorAsync(ex);
                        throw;
                    }
                }
            });
        }

        public async Task<bool> MarkMessageAsReadAsync(string messageId, string userId, string chatId)
        {
            var message = await _repository.Find(m => m.Id == messageId);
            // Nếu không tìm thấy tin nhắn thì lấy tin nhắn gần trước nhất

            if (message == null)
            {
                message = await _repository.Find(m => m.ChatId == chatId && m.SentAt < DateTime.UtcNow, orderBy: q => q.OrderByDescending(m => m.SentAt));
            }
            if (message.MessageReaders == null)
            {
                message.MessageReaders = new List<MessageReader>();
            }

            if (!message.MessageReaders.Any(mr => mr.ReaderId == userId))
            {
                message.MessageReaders.Add(new MessageReader
                {
                    MessageId = messageId,
                    ReaderId = userId,
                    ReadAt = DateTime.UtcNow
                });

                await _repository.Update(message);
                await _unitOfWork.SaveChangeAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> HasMessageAccess(string messageId, string userId)
        {
            var message = await _repository.Find(m => m.Id == messageId, include: m => m.Include(c => c.Chat).ThenInclude(c => c.UserChats));
            return message != null && message.Chat.UserChats.Any(uc => uc.UserId == userId);
        }

        private async Task<UserChat> GetUserChatAsync(string chatId, string userId)
        {
            return await _userChatRepository.Find(uc => uc.ChatId == chatId && uc.UserId == userId, include: uc => uc.Include(c => c.User));
        }

        private List<Attachment> CreateAttachments(IEnumerable<AttachmentCreateDto> attachmentDtos)
        {
            return attachmentDtos.Select(a => new Attachment
            {
                Id = a.UploadId!,
                FileUrl = a.FileUrl,
                FileName = a.FileName,
                FileType = FileTypeHelper.GetFileTypeFromMimeType(a.FileType),
                FileSize = a.FileSize
            }).ToList();
        }

        private async Task LogErrorAsync(Exception ex)
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
        }

        //Thay vì xóa toàn bộ cache, bạn có thể thêm tin nhắn mới vào đầu danh sách cache hiện có
        private async Task UpdateMessagesInCache(MessageDto messageCreateDto)
        {
            string cacheKey = string.Format(ChatMessagesCacheKey, messageCreateDto.ChatId, 1, 20); // Mặc định thì lấy 20 tin nhắn đầu tiên
            var cachedMessages = await _cacheService.GetAsync<PagedResponse<List<MessageDto>>>(cacheKey);
            if (cachedMessages != null)
            {
                cachedMessages.Data.Insert(0, messageCreateDto); // Thêm tin nhắn mới vào đầu danh sách
                await _cacheService.SetAsync(cacheKey, cachedMessages, TimeSpan.FromMinutes(5));
            }
        }

        // Hàm cập nhật thôgn tin chat cuối cùng trong danh sách chat của mỗi người tham gia đoạn chat
        // Lặp qua tất cả các page của danh sách chat của mỗi người tham gia đoạn chat
        private async Task UpdateChatListInCache(string chatId, MessageDto messageCreateDto)
        {
            var userChats = await _userChatRepository.FindAll(uc => uc.ChatId == chatId);
            var participantIds = userChats.Select(uc => uc.UserId).ToList();
            foreach (var participantId in participantIds)
            {
                bool chatUpdated = false;
                int page = 1;
                int pageSize = 20;
                PagedResponse<List<ChatDto>> cachedChatList = null;
                List<ChatDto> cachedChatListDat = null;
                int maxPagesToCheck = 5;

                while (!chatUpdated && page <= maxPagesToCheck)
                {
                    string chatListCacheKey = string.Format(ChatListCacheKey, participantId, page, pageSize);
                    cachedChatList = await _cacheService.GetAsync<PagedResponse<List<ChatDto>>>(chatListCacheKey);
                    if (cachedChatList == null || !cachedChatList.Data.Any())
                    {
                        break;
                    }

                    cachedChatListDat = cachedChatList.Data;
                    var chatToUpdate = cachedChatListDat.FirstOrDefault(c => c.Id == messageCreateDto.ChatId);
                    if (chatToUpdate != null)
                    {
                        // Xóa chat cũ khỏi danh sách
                        cachedChatListDat.Remove(chatToUpdate);
                        await _cacheService.SetAsync(chatListCacheKey, cachedChatList, TimeSpan.FromMinutes(2));

                        // Cập nhật lại cache cho trang 1
                        string firstPageCacheKey = string.Format(ChatListCacheKey, participantId, 1, pageSize);
                        var firstPageChatList = await _cacheService.GetAsync<PagedResponse<List<ChatDto>>>(firstPageCacheKey);
                        if (firstPageChatList != null && firstPageChatList.Data.Any())
                        {
                            var firstPageChatListDat = firstPageChatList.Data;
                            chatToUpdate.Messages = new List<MessageDto> { messageCreateDto };
                            firstPageChatListDat.Insert(0, chatToUpdate);
                            await _cacheService.SetAsync(firstPageCacheKey, firstPageChatList, TimeSpan.FromMinutes(2));
                        }

                        chatUpdated = true;
                    }
                    page++;
                }
            }
        }

    }
}
