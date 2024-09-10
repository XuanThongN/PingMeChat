using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared.Model.Student
{
    public class StudentSearchModel : RequestDataTable
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
        public string? AcademicTermId { get; set; }
        public string? FacultyId { get; set; }
        public string? MajorId { get; set; }
        public string? ClassId { get; set; }
    }
}
