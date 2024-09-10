using AutoMapper;
using PingMeChat.CMS.Application.Feature.Service.Customers.Dto;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.Customers
{
    public class CustomerMapper : Profile
    {
        public CustomerMapper()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CustomerUpdateDto>().ReverseMap();
            CreateMap<Customer, CustomerCreateDto>().ReverseMap();
        }
    }
}
