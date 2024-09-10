using AutoMapper;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Indentity.RefreshTokens
{
    public class TokenMapper : Profile
    {
        public TokenMapper()
        {
            {
                CreateMap<RefreshToken, RefreshTokenDto>().ReverseMap();
            }
        }
    }
}
