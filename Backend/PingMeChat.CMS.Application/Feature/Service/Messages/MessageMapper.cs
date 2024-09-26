using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Messages
{
    public class MessageMapper : Profile
    {
        public MessageMapper()
        {
            // CreateMap<Message, MessageDto>().ReverseMap();
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                .ReverseMap();
            CreateMap<Message, MessageCreateDto>().ReverseMap();
            CreateMap<Message, MessageUpdateDto>().ReverseMap();
        }
    }
}
