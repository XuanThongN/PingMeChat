using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Users
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<Account, UserDto>().ReverseMap();
            CreateMap<Account, UserCreateDto>().ReverseMap();
            CreateMap<Account, UserUpdateDto>().ReverseMap();
        }
    }
}
