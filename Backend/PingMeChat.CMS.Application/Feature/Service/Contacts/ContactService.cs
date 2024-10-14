using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Users;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.Entities.Module;
using PingMeChat.CMS.Application.App.Repositories;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using PingMeChat.CMS.Application.Feature.Service.Contacts.Dto;
using PingMeChat.CMS.Entities.Feature;
using Octokit;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.Shared.Enum;
using Microsoft.AspNetCore.Http;
using PingMeChat.CMS.Application.Feature.Services.RedisCacheServices;
using PingMeChat.CMS.Application.Feature.ChatHubs;

namespace PingMeChat.CMS.Application.Feature.Service.Contacts
{
    public interface IContactService : IServiceBase<Contact, ContactCreateDto, ContactUpdateDto, ContactDto, IContactRepository>
    {
        Task<IEnumerable<ContactDto>> GetUserContacts(string userId);
        // Get tất cả id liên hệ của user
        Task<Dictionary<String, ContactStatus>> GetAllContactIds(string userId);
        Task<ContactDto> SendFriendRequest(string userId, string contactId);
        Task<ContactDto> AcceptFriendRequest(string userId, string contactId);
        Task<bool> CancelFriendRequest(string userId, string contactId);
        Task<List<string>> GetAllFriendContactIds(string userId);
        // Get tất cả id liên hệ của user kèm theo status online/offline
        Task<Dictionary<string, bool>> GetAllFriendContactStatuses(string userId);
    }
    public class ContactService : ServiceBase<Contact, ContactCreateDto, ContactUpdateDto, ContactDto, IContactRepository>, IContactService
    {
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly IContactRepository _contactRepository;
        private readonly ICacheService _cacheService; // Sử dụng cache để lưu trữ thông tin liên hệ
        private readonly IRedisConnectionManager _redisConnectionManager; // Sử dụng Redis để lưu trữ các connection tới SignalR
        private const string UserContactsCacheKey = "UserContacts_{0}"; // userId
        private const string GetAllFriendContactIdsCacheKey = "GetAllFriendContactIds_{0}"; // userId
        public ContactService(IContactRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
             ILogErrorRepository logErrorRepository,
             ICacheService cacheService,
                IRedisConnectionManager redisConnectionManager
             ) : base(repository, unitOfWork, mapper, uriService)
        {
            _contactRepository = repository;
            _logErrorRepository = logErrorRepository;
            _cacheService = cacheService;
            _redisConnectionManager = redisConnectionManager;
        }
        public async Task<IEnumerable<ContactDto>> GetUserContacts(string currentUserId)
        {
            return await _cacheService.GetOrCreateAsync(string.Format(UserContactsCacheKey, currentUserId), async () =>
            {
                var contacts = await _contactRepository.GetContactsByUserIdAsync(currentUserId);

                return contacts.Select(c => new ContactDto
                {
                    Id = c.Id,
                    // Xác định xem userId hiện tại là người gửi hay người nhận liên hệ
                    UserId = c.UserId,
                    ContactUserId = c.ContactUserId,
                    ContactUser = _mapper.Map<AccountDto>(c.ContactUser),
                    User = _mapper.Map<AccountDto>(c.User),
                    Status = GetContactStatus(c, currentUserId),
                    CreatedDate = c.CreatedDate ?? DateTime.MinValue,
                }).ToList();
            }, TimeSpan.FromMinutes(10));
        }

        public async Task<Dictionary<String, ContactStatus>> GetAllContactIds(string userId)
        {
            var contacts = await _contactRepository.FindAll(c => c.UserId == userId || c.ContactUserId == userId);
            // Kiểm tra xem userId hiện tại là người gửi hay người nhận liên hệ và trả về status
            // Và nếu userId hiện tại là người gửi thì trả về contactUserId, ngược lại trả về userId 
            // Còn status trường hợp Pending mà userId hiện tại == userId thì là người gửi và status sẽ là Requested còn ngược lại là Pending
            return contacts.Select(c =>
            {
                var contactUserId = c.UserId == userId ? c.ContactUserId : c.UserId;
                var status = c.Status == ContactStatus.Pending && c.UserId == userId ? ContactStatus.Requested : c.Status;
                return new { contactUserId, status };
            }).ToDictionary(c => c.contactUserId, c => c.status);
        }

        // Hàm Gửi yêu cầu kết bạn
        public async Task<ContactDto> SendFriendRequest(string userId, string contactId)
        {
            var contact = await _contactRepository.Find(c => (c.UserId == userId && c.ContactUserId == contactId)
                                                                || c.ContactUserId == userId && c.UserId == contactId
                                                                && c.Status == ContactStatus.Pending);
            if (contact != null)
            {
                throw new AppException("Yêu cầu kết bạn đã tồn tại.", StatusCodes.Status400BadRequest);
            }
            contact = new Contact
            {
                UserId = userId,
                ContactUserId = contactId,
                Status = ContactStatus.Pending
            };
            contact = await _contactRepository.Add(contact);
            await _unitOfWork.SaveChangeAsync();

            // Invalidate contacts cache for both users
            await _cacheService.RemoveAsync(string.Format(UserContactsCacheKey, userId));
            await _cacheService.RemoveAsync(string.Format(UserContactsCacheKey, contactId));

            return _mapper.Map<ContactDto>(contact);
        }

        public async Task<ContactDto> AcceptFriendRequest(string userId, string contactId)
        {
            var contact = await _contactRepository.Find(c => (c.UserId == userId && c.ContactUserId == contactId)
                                                                || c.ContactUserId == userId && c.UserId == contactId
                                                                && c.Status == ContactStatus.Pending);

            if (contact == null)
            {
                throw new AppException("Không tìm thấy yêu cầu kết bạn.", StatusCodes.Status404NotFound);
            }

            if (contact.Status != ContactStatus.Pending)
            {
                throw new AppException("Yêu cầu kết bạn không hợp lệ.", StatusCodes.Status400BadRequest);
            }

            contact.Status = ContactStatus.Accepted;
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<ContactDto>(contact);
        }

        public async Task<bool> CancelFriendRequest(string userId, string contactId)
        {
            var contact = await _contactRepository.Find(c => (c.UserId == userId && c.ContactUserId == contactId)
                                                                || c.ContactUserId == userId && c.UserId == contactId
                                                                && c.Status == ContactStatus.Pending);

            if (contact == null)
            {
                throw new AppException("Không tìm thấy yêu cầu kết bạn.", StatusCodes.Status404NotFound);
            }

            if (contact.Status == ContactStatus.Accepted)
            {
                throw new AppException("Không thể hủy kết bạn đã được chấp nhận.", StatusCodes.Status400BadRequest);
            }

            await _contactRepository.SoftDelete(contact);
            await _unitOfWork.SaveChangeAsync();

            // Invalidate contacts cache for both users
            await _cacheService.RemoveAsync(string.Format(UserContactsCacheKey, userId));
            await _cacheService.RemoveAsync(string.Format(UserContactsCacheKey, contactId));
            return true;
        }

        public async Task<List<string>> GetAllFriendContactIds(string userId)
        {
            var cacheKey = string.Format(GetAllFriendContactIdsCacheKey, userId);
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var contacts = await _contactRepository.FindAll(c => (c.UserId == userId || c.ContactUserId == userId) && c.Status == ContactStatus.Accepted);
                return contacts.Select(c => c.UserId == userId ? c.ContactUserId : c.UserId).ToList();
            }, TimeSpan.FromHours(10));
        }

        public async Task<Dictionary<string, bool>> GetAllFriendContactStatuses(string userId)
        {
            var friendIds = await GetAllFriendContactIds(userId);
            var result = new Dictionary<string, bool>();
            foreach (var friendId in friendIds)
            {
                var friendConnectionIds = await _redisConnectionManager.GetConnectionsAsync(friendId);
                result.Add(friendId, friendConnectionIds.Any());
            }
            return result;
        }

        //Hàm để lấy Status chính xác của contact
        private ContactStatus GetContactStatus(Contact contact, string userId)
        {
            if (contact.Status == ContactStatus.Pending && contact.UserId == userId)
            {
                return ContactStatus.Requested;
            }
            return contact.Status;
        }

    }
}
