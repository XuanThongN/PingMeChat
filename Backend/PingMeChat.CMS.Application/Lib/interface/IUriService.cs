using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Lib
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationQuery filter, string route);
    }
}
