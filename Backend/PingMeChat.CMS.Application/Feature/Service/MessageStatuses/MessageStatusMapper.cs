using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.MessageStatuses.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.MessageStatuses
{
    public class MessageStatusMapper : Profile

    {
        public MessageStatusMapper()
        {
            CreateMap<MessageStatus, MessageStatusDto>().ReverseMap();
            CreateMap<MessageStatus, MessageStatusCreateDto>().ReverseMap();
            CreateMap<MessageStatus, MessageStatusUpdateDto>().ReverseMap();
        }
    }
}
