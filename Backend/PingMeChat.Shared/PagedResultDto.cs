using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public class PagedResultDto<T>
    {
        public ResultDataTable<T> Result { get; set; }
        public string? TargetUrl { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; }

        public PagedResultDto(ResultDataTable<T> result, string? targetUrl, bool success, string? error, int statusCode)
        {
            Result = result;
            TargetUrl = targetUrl;
            Success = success;
            Error = error;
            StatusCode = statusCode;
        }
    }
    public class ResultDataTable<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T> Data { get; set; }

        public ResultDataTable(int draw, int recordsTotal, int recordsFiltered, List<T> data)
        {
            RecordsTotal = recordsTotal;
            RecordsFiltered = recordsFiltered;
            Draw = draw;
            Data = data;
        }
    }
    public class RequestDataTable
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
        public int Draw { get; set; } = 1;
        public int Start { get; set; }
        public int Length { get; set; } = 10;
        public int CurrentPage => (Start / Length) + 1;
    }
}
