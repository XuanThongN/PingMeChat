using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Common.ExcelModel.Exports
{
    public class BaseExcelExportModel
    {
        public string VariableName { get; set; }
        public string HeaderName { get; set; }
        public object VariableValue { get; set; }
    }
}
