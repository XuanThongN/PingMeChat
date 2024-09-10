using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Menus.Dto;
using PingMeChat.CMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Menus
{
    public class MenuMapper : Profile
    {
        public MenuMapper()
        {
            CreateMap<UserMenu, UserMenuDto>().ReverseMap();
            CreateMap<RoleMenu, RoleMenuDto>().ReverseMap();
            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Menu, MenuCreateDto>().ReverseMap();
            CreateMap<Menu, MenuUpdateDto>().ReverseMap();
        }
    }
}
