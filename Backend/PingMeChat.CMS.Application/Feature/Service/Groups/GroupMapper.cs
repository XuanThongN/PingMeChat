using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Groups.Dto;
using PingMeChat.CMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Groups
{
    public class GroupMapper : Profile
    {
        public GroupMapper()
        {
            CreateMap<Group, GroupDto>().ReverseMap();
            CreateMap<Group, GroupCreateDto>().ReverseMap();
            CreateMap<Group, GroupUpdateDto>().ReverseMap();
        }
    }
}
