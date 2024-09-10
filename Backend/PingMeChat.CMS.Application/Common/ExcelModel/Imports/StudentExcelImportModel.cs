using PingMeChat.Shared.Enum;

namespace PingMeChat.CMS.Application.Common.ExcelModel.Imports
{
    public class StudentExcelImportModel : BaseExcelImportModel
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAccount { get; set; }
        public string Email { get; set; }
        public Sex Sex { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public EducationalSystem EducationalSystem { get; set; }

        public string AcademicTermId { get; set; }
        // khoa
        public string FacultyId { get; set; }
        // nganh
        public string MajorId { get; set; }
        // lop
        public string ClassRoomId { get; set; }
    }
}
