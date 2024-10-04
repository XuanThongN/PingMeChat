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

namespace PingMeChat.CMS.Application.Feature.Service.Messages
{
    public interface IMessageService : IServiceBase<Message, MessageCreateDto, MessageUpdateDto, MessageDto, IMessageRepository>
    {
        Task<MessageDto> SendMessageAsync(MessageCreateDto messageCreateDto);
        Task<PagedResponse<List<MessageDto>>> GetChatMessagesAsync(string chatId, int pageNumber, int pageSize, string route = null);
    }

    public class MessageService : ServiceBase<Message, MessageCreateDto, MessageUpdateDto, MessageDto, IMessageRepository>, IMessageService
    {
        private readonly IUserChatRepository _userChatRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly IChatHubService _chatHubService;
        private readonly IAccountRepository _accountRepository;
        private readonly IFCMService _fcmService;
        public MessageService(
            IMessageRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IUserChatRepository userChatRepository,
            ILogErrorRepository logErrorRepository,
            IChatHubService chatHubService,
            IAccountRepository accountRepository,
            IFCMService fcmService)
            : base(repository, unitOfWork, mapper, uriService)
        {
            _userChatRepository = userChatRepository;
            _logErrorRepository = logErrorRepository;
            _chatHubService = chatHubService;
            _accountRepository = accountRepository;
            _fcmService = fcmService;
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
            return await Pagination(
                pageNumber,
                pageSize,
                predicate: m => m.ChatId == chatId,
                orderBy: q => q.OrderByDescending(m => m.SentAt),
                include: m => m.Include(o => o.Sender),
                route: route
                );
        }
        public async Task<MessageDto> SendMessageAsync(MessageCreateDto messageCreateDto)
        {
            // content = SanitizeContent(content);

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                // Gói toàn bộ trong một đơn vị thử lại
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var userChat = await _userChatRepository.Find(uc => uc.ChatId == messageCreateDto.ChatId
                                                                                && uc.UserId == messageCreateDto.SenderId, include: uc => uc.Include(c => c.User));
                        if (userChat == null)
                        {
                            throw new AppException("Người dùng không thuộc đoạn chat này", 403);
                        }

                        var attachments = new List<Attachment>();
                        if (messageCreateDto.Attachments.Any())
                            attachments = messageCreateDto.Attachments.Select(a =>
                            {
                                return new Attachment
                                {
                                    FileUrl = a.FileUrl,
                                    FileName = a.FileName,
                                    FileType = FileTypeHelper.GetFileTypeFromMimeType(a.FileType),
                                    FileSize = a.FileSize
                                };
                            }).ToList();
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
                        // Thông báo tin nhắn mới tới những người tham gia đoạn chat
                        await _chatHubService.SendMessageAsync(result);

                        // Send push notification to all recipients except sender 
                        var recipients = await _userChatRepository.FindAll(uc => uc.ChatId == messageCreateDto.ChatId
                                                                                    && uc.UserId != messageCreateDto.SenderId,
                                                                                    include: uc => uc.Include(c => c.User)
                                                                                    );
                        foreach (var recipient in recipients)
                        {
                            if (!string.IsNullOrEmpty(recipient.User.FCMToken))
                            {
                                var data = new Dictionary<string, string>
                                {
                                    { "chatId", messageCreateDto.ChatId },
                                    { "messageId", message.Id },
                                    { "senderId", messageCreateDto.SenderId }
                                };
                                await _fcmService.SendNotificationAsync(recipient.User.FCMToken, message.Sender.FullName! ?? message.Sender.UserName, message.Content ?? $"{message.Sender.FullName} sent a new message", data);
                            }
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
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
            });
        }


        // Hàm để loại bỏ các thẻ HTML và mã hóa các ký tự đặc biệt
        private string SanitizeContent(string content)
        {
            // Remove any potentially dangerous HTML tags
            content = Regex.Replace(content, @"<.*?>", string.Empty);

            // Encode special characters
            content = System.Web.HttpUtility.HtmlEncode(content);

            return content;
        }
    }
}
