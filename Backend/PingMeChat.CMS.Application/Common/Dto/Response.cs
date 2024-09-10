using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Common.Dto
{
    public class Response<T>
    {
        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
        public Response()
        {
        }
        public Response(T data)
        {
            Data = data;
            Succeeded = true;
            Errors = null;
            Message = string.Empty;
        }
    }
}
