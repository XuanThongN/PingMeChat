using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Common.Exceptions
{
    public class ExcelException : Exception
    {
        public string Message { get; set; }
        public object Data { get; set; }
        public int Status { get; set; }

        public ExcelException() : base()
        {
        }

        public ExcelException(string message) : base(message)
        {
            Message = message;
            Status = 500; // Mặc định là lỗi server error
        }
        public ExcelException(string message, int status = 500) : base(message)
        {
            Message = message;
            Status = status;
        }

        public ExcelException(string message, Exception innerException) : base(message, innerException)
        {
            Message = message;
            Status = 500;
        }

        public ExcelException(string message, object data, int status = 500) : base(message)
        {
            Message = message;
            Data = data;
            Status = status;
        }
    }
}
