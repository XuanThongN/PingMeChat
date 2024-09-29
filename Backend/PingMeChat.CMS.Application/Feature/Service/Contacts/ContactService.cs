﻿using AutoMapper;
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
    }
    public class ContactService : ServiceBase<Contact, ContactCreateDto, ContactUpdateDto, ContactDto, IContactRepository>, IContactService
    {
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly IContactRepository _contactRepository;
        public ContactService(IContactRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
             ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _contactRepository = repository;
            _logErrorRepository = logErrorRepository;
        }
        public async Task<IEnumerable<ContactDto>> GetUserContacts(string currentUserId)
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
                //Status = c.Status
            }).ToList();
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

            return true;
        }

    }
}
