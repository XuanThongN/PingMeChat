using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PingMeChat.Shared.Enum
{
    public enum TypeSemester
    {
        [Display(Name = "Fall")]
        Fall = 1,
        [Display(Name = "Summer")]
        Summer = 2,
        [Display(Name = "Spring")]
        Spring = 3,
    }
    public enum SchoolYearType
    {
        [Display(Name = "Toàn khóa")]
        EntireClass = 1,
        [Display(Name = "Năm học")]
        SchoolYear = 2,
        [Display(Name = "Học kỳ")]
        Semester = 3,
    }
    public enum StatusProcessing
    {
        [Display(Name = "Tiếp nhận")]
        Receive = 1,
        [Display(Name = "Đã xử lý")]
        Completed = 2,
        [Display(Name = "Đã hủy")]
        Canceled = 3,
        [Display(Name = "Đang xử lý")]
        Processing = 4
    }
    public enum FormTypeEnum
    {
        [Display(Name = "Đơn xin cấp bảng điểm")]
        PingMeChat_DXCBD = 1,
        [Display(Name = "Đơn xin cấp bằng tốt nghiệp")]
        PingMeChat_XCBTN = 2,
        [Display(Name = "Đơn xin cấp chứng chỉ")]
        PingMeChat_XCCC = 3,
        [Display(Name = "Đơn xin cấp giấy chứng nhận")]
        PingMeChat_XCGCN = 4,
        [Display(Name = "Đơn xin cấp giấy xác nhận")]
        PingMeChat_XCGXC = 5,
        [Display(Name = "Đơn xin cấp học bổng")]
        PingMeChat_XCHB = 6,
        [Display(Name = "Đơn xin cấp học phí")]
        PingMeChat_XCHP = 7,
        [Display(Name = "Đơn xin cấp hộ chiếu")]
        PingMeChat_XCHC = 8,
        [Display(Name = "Đơn xin cấp thẻ sinh viên")]
        PingMeChat_DXLLTSV = 9,
        [Display(Name = "Đơn xin cấp thẻ thư viện")]
        PingMeChat_XCTTV = 10,
        [Display(Name = "Đơn đăng ký học phần")]
        PingMeChat_DDKHP = 11,
        [Display(Name = "Đơn xác nhận là sinh viên đang học tại trường")]
        PingMeChat_DXNLSVDHTT = 12,
    }
    public enum Sex
    {
        [Display(Name = "Nam")]
        Male = 1,
        [Display(Name = "Nữ")]
        Female = 2,
        [Display(Name = "Khác")]
        Other = 3,

    }
    public enum StatusCourse
    {
        [Display(Name = "Chưa đến hạn")]
        Undue = 1,
        [Display(Name = "Hoạt động")]
        Active = 2,
        [Display(Name = "Đã hủy")]
        Cancelled = 3,
        [Display(Name = "Đã hết hạn")]
        Expired = 4,
    }

    public enum Status
    {
        [Display(Name = "Hoạt động")]
        Active = 1,
        [Display(Name = "Đã khóa")]
        Cancelled = 2,
    }
    public enum LoaiChucVu
    {
        [Display(Name = "Hiệu trưởng")]
        HIEU_TRUONG = 1,
        [Display(Name = "Phó hiệu trưởng")]
        PHO_HIEU_TRUONG = 2,
        [Display(Name = "Trưởng khoa")]
        TRUONG_KHOA = 3,
        [Display(Name = "Trưởng ngành")]
        TRUONG_NGANH = 4,
        [Display(Name = "Chủ nhiệm lớp")]
        CHU_NHIEM_LOP = 5,
        [Display(Name = "Kế toán")]
        KE_TOAN = 6,
        [Display(Name = "Nhân viên")]
        NHAN_VIEN = 7,
    }
    public enum StatusNotification
    {
        [Display(Name = "Đã xem")]
        Watched = 1,
        [Display(Name = "Chưa xem")]
        NotSeen = 2,
        [Display(Name = "Đang xử lý")]
        Processing = 3,
        [Display(Name = "Thành công")]
        Processed = 4,
        [Display(Name = "Thất bại")]
        Error = 5,
    }
    public enum TypeNotification
    {
        [Display(Name = "Đơn đăng ký")]
        RegistrationForm = 1,
        [Display(Name = "Công văn")]
        OfficialDocument = 2,
        [Display(Name = "Yêu cầu hỗ trợ")]
        Support = 3,
    }
    public enum EducationalSystem
    {
        [Display(Name = "Chính quy")]
        RegularProgram = 1,
        [Display(Name = "Liên thông")]
        ArticulatedProgram = 2
    }
    public enum DayOfTheWeek // thứ học
    {
        [Display(Name = "Thứ 2")]
        Monday = 1,
        [Display(Name = "Thứ 3")]
        Tuesday = 2,
        [Display(Name = "Thứ 4")]
        Wednesday = 3,
        [Display(Name = "Thứ 5")]
        Thursday = 4,
        [Display(Name = "Thứ 6")]
        Friday = 5,
        [Display(Name = "Thứ 7")]
        Saturday = 6,
        [Display(Name = "Chủ nhật")]
        Sunday = 7
    }
    public enum AttemptExamGrade
    {
        [Display(Name = "Lần 1")]
        FirstAttempt = 1,
        [Display(Name = "Lần 2")]
        SecondAttempt = 2,
    }

    public enum PositionType
    {
        [Display(Name = "Sinh viên")]
        IsStudent = 1,
        [Display(Name = "Cán bộ nhân viên")]
        IsStaff = 2,
    }
    public enum FileType
    {
        ACADEMICTERM = 1,
        STUDENT = 2,
        STAFF = 3,

    }

    public enum LogEventLevel
    {
        [Display(Name = "Log hệ thống")]
        System = 1,
        [Display(Name = "Dữ liệu")]
        DataLog = 2,
    }

    public enum DDKHPadditionalInformation
    {
        [Display(Name = "Đăng ký lần đầu")]
        FirstRegistration = 1,
        [Display(Name = "Đăng ký bổ sung")]
        AdditionalSubscriptions = 2,
        [Display(Name = "Đăng ký học lại")]
        SignUpToStudyAgain = 3,
        [Display(Name = "Đăng ký học vượt")]
        SignUpForAPass = 4,
        [Display(Name = "Đăng ký học cải thiện điểm")]
        RegisterToImproveYourScore = 5,

    }
    public enum RoleType
    {
        SUPPERADMIN = 1,
        ADMIN = 2,
        BASIC = 3
    }

    public enum TypeTeacher
    {
        [Display(Name = "hợp đồng")] // giảng viên được thuê bên ngoài hoặc thời vụ
        Unify = 1,
        [Display(Name = "chính thức")] // giảng viên trong nhà trường
        Official = 2,
    }
    public enum StatusWorking
    {
        [Display(Name = "Hoạt động")]
        Active = 1,
        [Display(Name = "Đã nghỉ làm")]
        Cancel = 2,
    }

    public enum StatusFeedback
    {
        [Display(Name = "Chờ phản hồi")]
        Waiting = 1,
        [Display(Name = "Đã phản hồi")]
        Feedbacked = 2,
    }

    public enum FeedbackType
    {
        [Display(Name = "Biểu mẫu - đơn từ")]
        FeedbackType1 = 1,
    }

    public enum StatisticalData
    {
       Year = 1, // 
       Quarter = 2, // quý
       Day = 3,
    }
}
