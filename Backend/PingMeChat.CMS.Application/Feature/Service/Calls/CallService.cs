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
using PingMeChat.CMS.Application.Feature.Service.Calls.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.Calls
{
    public interface ICallService : IServiceBase<Call, CallCreateDto, CallUpdateDto, CallDto, ICallRepository>
    {
    }
    public class CallService : ServiceBase<Call, CallCreateDto, CallUpdateDto, CallDto, ICallRepository>, ICallService
    {
        private readonly ICallRepository _callRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        public CallService(ICallRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUriService uriService,
            ICallRepository callRepository,
         ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _callRepository = callRepository;
            _logErrorRepository = logErrorRepository;
        }

    }


}
