using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public class RequestData<T>
    {
        public T Data { get; set; }
        public string? IdClient { get; set; }
        
        // phân trang
    }
}
