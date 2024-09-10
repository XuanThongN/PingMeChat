using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public static class ReportHelper
    {
        public static string GetReportPath(object obj, string param)
        {
            var type = obj.GetType();
            var reportPath = type.GetProperties().FirstOrDefault(x => x.Name == param).GetValue(obj);
            return reportPath.ToString();

        }
    }
}
