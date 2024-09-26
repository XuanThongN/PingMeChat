using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Users;

namespace PingMeChat.CMS.Application.Feature.Service.Attachments
{
    public class AttachmentMapper : Profile
    {
        public AttachmentMapper()
        {
            CreateMap<Attachment, AttachmentDto>().ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FileUrl)).ReverseMap();
            CreateMap<Attachment, AttachmentCreateDto>().ReverseMap();
            CreateMap<Attachment, AttachmentUpdateDto>().ReverseMap();
        }
    }
}
