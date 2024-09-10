using Microsoft.AspNetCore.WebUtilities;
using PingMeChat.CMS.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingMeChat.CMS.Application.Domain.Dto;

namespace PingMeChat.CMS.Application.Lib
{
    public class UriService : IUriService
    {
        public readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }
        // https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/

        public Uri GetPageUri(PaginationQuery filter, string route)
        {
            var _endPointUri = new Uri(string.Concat(_baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(_endPointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}
