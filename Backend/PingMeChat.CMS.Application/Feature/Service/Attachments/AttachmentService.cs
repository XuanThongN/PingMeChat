using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;

namespace PingMeChat.CMS.Application.Feature.Service.CallParticipants
{
    public interface IAttachmentService : IServiceBase<Attachment, AttachmentCreateDto, AttachmentUpdateDto, AttachmentDto, IAttachmentRepository>
    {
    }
    public class AttachmentService : ServiceBase<Attachment, AttachmentCreateDto, AttachmentUpdateDto, AttachmentDto, IAttachmentRepository>, IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        public AttachmentService(IAttachmentRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUriService uriService,
                        IAttachmentRepository attachmentRepository,
         ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _attachmentRepository = attachmentRepository;
            _logErrorRepository = logErrorRepository;
        }

    }


}
