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

namespace PingMeChat.CMS.Application.Feature.Service.Contacts
{
    public interface IContactService : IServiceBase<Contact, ContactCreateDto, ContactUpdateDto, ContactDto, IContactRepository>
    {
        Task<IEnumerable<ContactDto>> GetUserContacts(string userId);
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

    }
}
