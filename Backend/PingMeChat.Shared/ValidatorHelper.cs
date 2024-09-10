using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public static class ValidatorHelper
    {
        public static bool IsEmail(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            return input.Contains("@") && input.IndexOf(".", input.IndexOf("@")) > input.IndexOf("@");
        }
    }
}
