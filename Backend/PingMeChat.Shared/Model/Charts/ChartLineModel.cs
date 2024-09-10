using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared.Model.Charts
{
    public class ChartLineModel
    {
        public List<string> Labels  = new List<string>();
        public List<int> DataBefore = new List<int>();
        public List<int> DataAfter = new List<int>();

    }
    public class DataChartLine
    {
        public TypeDataChartLine TypeData { get; set; }
        public List<int> Data  = new List<int>(); 
    }

    public enum TypeDataChartLine{
        [Display(Name = "Kỳ trước")]
        BeforData = 0,
        [Display(Name = "Kỳ sau")]
        AfterData = 1,
    }
}
