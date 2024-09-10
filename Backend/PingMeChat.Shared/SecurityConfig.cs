using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public static class SecurityConfig
    {
        public static IConfiguration configuration;
        public static string Security_key
        {
            get{
                var key = configuration["SecurityKey"];
                return key != null ? key : string.Empty;
            } 
        }
    }
}
