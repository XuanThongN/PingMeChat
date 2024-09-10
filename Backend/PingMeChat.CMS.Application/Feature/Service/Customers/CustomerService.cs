using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.Customers.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Customers
{
    public interface ICustomerService : IServiceBase<Customer, CustomerCreateDto, CustomerUpdateDto, CustomerDto, ICustomerRepository>
    {
    }
    public class CustomerService : ServiceBase<Customer, CustomerCreateDto, CustomerUpdateDto, CustomerDto, ICustomerRepository>, ICustomerService
    {
        public CustomerService(ICustomerRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IUriService uriService) : base(repository, unitOfWork, mapper, uriService)
        {
        }
    }
}
