using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.UserChats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Users;

namespace PingMeChat.CMS.Application.Feature.Service.UserChats
{
    public class UserChatMapper : Profile
    {
        public UserChatMapper()
        {
            //CreateMap<UserChat, UserChatDto>();
            CreateMap<UserChat, UserChatDto>().ForMember(
                dest => dest.UserDto, opt => opt.MapFrom(src => src.User
                )).ReverseMap();
            CreateMap<UserChat, UserChatCreateDto>().ReverseMap();
            CreateMap<UserChat, UserChatUpdateDto>().ReverseMap();
        }
    }
}
