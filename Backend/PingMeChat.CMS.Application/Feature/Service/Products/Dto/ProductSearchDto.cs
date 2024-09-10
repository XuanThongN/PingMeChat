using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Products.Dto
{
    public class ProductSearchDto : RequestDataTable
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
