using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Customers.Dto
{
    public class CustomerDto : ReadDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
