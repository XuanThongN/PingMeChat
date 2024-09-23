using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Contacts.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.Contacts   
{
    public class ContactMapper : Profile
    {
        public ContactMapper()
        {
            //CreateMap<Contact, ContactDto>().ReverseMap();
            CreateMap<Contact, ContactDto>()
                .ForMember(d => d.ContactUser, o => o.MapFrom(src => src.ContactUser))
                .ForMember(d => d.User, o => o.MapFrom(src => src.User));
            CreateMap<Contact, ContactCreateDto>().ReverseMap();
            CreateMap<Contact, ContactUpdateDto>().ReverseMap();
        }
    }
}
