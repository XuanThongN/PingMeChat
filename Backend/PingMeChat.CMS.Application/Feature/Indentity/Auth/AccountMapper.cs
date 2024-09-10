using AutoMapper;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<Account, AccountDto>().ReverseMap();
        }
    }
}
