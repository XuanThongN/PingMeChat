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
using PingMeChat.CMS.Application.Feature.Service.CallParticipants.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.CallParticipants
{
    public interface ICallParticipantService : IServiceBase<CallParticipant, CallParticipantCreateDto, CallParticipantUpdateDto, CallParticipantDto, ICallParticipantRepository>
    {
    }
    public class CallParticipantService : ServiceBase<CallParticipant, CallParticipantCreateDto, CallParticipantUpdateDto, CallParticipantDto, ICallParticipantRepository>, ICallParticipantService
    {
        private readonly ICallParticipantRepository _callParticipantRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        public CallParticipantService(ICallParticipantRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUriService uriService,
                    ICallParticipantRepository callParticipantRepository,
         ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _callParticipantRepository = callParticipantRepository;
            _logErrorRepository = logErrorRepository;
        }

    }


}
