using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared.Utils
{
    public static class Message
    {
        public static class Error
        {
            #region main
            public const string CreateError = "Tạo mới không thành công {0}";
            public const string UpdatedError = "Cập nhật không thành công {0}";
            public const string DeletedError = "Xóa không thành công {0}";
            public const string Existed = "Dữ liệu đã tồn tại";
            public const string IsExisted = "{0} đã tồn tại trong hệ thống";
            public const string NotExisted = "Dữ liệu không tồn tại";
            public const string NotActive = "Dữ liệu không hoạt động";
            public const string NotAllow = "Không được phép";
            public const string NotValid = "Dữ liệu không hợp lệ";
            public const string InvalidRequest = "Yêu cầu không hợp lệ, vui lòng kiểm tra lại thông tin";
            public const string LoginFail = "Đăng nhập không thành công";
            public const string LogoutFail = "Đăng xuất thất bại";
            public const string LockAccountFail = "Khóa tài khoản thất bại";
            public const string UnLockAccountFail = "Mở khóa tài khoản thất bại";
            public const string NotFound = "Không tìm thấy {0} trong hệ thống";
            public const string PasswordDontMatch = "Mật khẩu không trùng khớp";
            public const string ChangePasswordFail = "Thay đổi mật khẩu không thành công";
            #endregion

            #region token
            public static class Token
            {
                public const string ErrorOccurred = "Đã xảy ra lỗi khi làm mới mã thông báo";
            }
            #endregion

            #region feature
            public static class User
            {
                public const string NotExisted = "Người dùng không tồn tại";
            }
            public static class PlayProcess
            {
                public const string UnableToStart = "Không thể bắt đầu phiên chơi cho bàn hiện tại";
            }
            #endregion
        }
        public static class Exception
        {
            public const string Existed = "{0} đã tồn tại trong hệ thống";
            public const string NotExits = "{0} không tồn tại trong hệ thống";
            public const string NotIncorrect = "{0} không chính xác";
            public const string IsLocked = "Tài khoản này đã bị khóa";
        }
        public static class Success
        {
            #region main
            public const string CreateCompleted = "Tạo mới thành công {0}";
            public const string UpdatedCompleted = "Cập nhật thành công {0}";
            public const string DeletedCompleted = "Xóa thành công {0}";
            public const string ReadedCompleted = "Dữ liệu {0}";
            public const string ReadedAllCompleted = "Danh sách {0}";
            public const string ChangeStatusCompleted = "Thay đổi trạng thái thành công {0}";
            #endregion

            #region auth
            public static class Auth
            {
                public const string RegisterCompleted = "Đăng ký tài khoản thành công";
                public const string LoginCompleted = "Đăng nhập thành công";
                public const string LogoutCompleted = "Đăng xuất thành công";
                public const string LockAccountCompleted = "Khóa tài khoản thành công";
                public const string UnLockAccountCompleted = "Mở khóa tài khoản thành công";
                public const string ChangePasswordCompleted = "Thay đổi mật khẩu thành công";

            }
            #endregion
            #region feature
            public static class BidaTable
            {
                public const string Created = "Tạo";
                public const string Updated = "Đã cập nhật thành công chủ đề";
                public const string Deleted = "Đã xóa thành công chủ đề";
                public const string Readed = "Dữ liệu chủ đề";
                public const string ReadedAll = "Danh sách bàn bida";
                public const string GetEmpties = "Danh sách bàn bida còn trống";
                public const string GetTotalWithStatus = "Danh sách số lượng trạng thái của bàn bida";
            }
            public static class PlayProcess
            {
                public const string Start = "Đã bắt đầu phiên chơi";
                public const string Payment = "Đã thanh toán thành công";
                public const string Debt = "Đã ghi nợ bàn {0} cho khách hàng {1}";
                public const string AddProductSession = "Đã thêm các sản phẩm dùng thêm trong quá trình chơi";
                public const string ChangeTable = "Bạn đã chuyển bàn thành công";
                public const string SplitHour = "Bạn đã tách giờ chơi cũ, chuyển sang giờ chơi mới trên bàn hiện tại";
                public const string GetAllProductSession = "Danh sách dịch vụ hiện tại đang sử dụng trên mã bàn {0}";

            }
            #endregion
        }

        public static class Info
        {
            public const string InFo01 = "Email người dùng";
            public const string InFo02 = "Tài khoản hoặc mật khẩu";
            public const string InFo03 = "Mã thông báo truy cập mới được tạo cho người dùng {0}";
        }
        public static class Warning
        {
            public const string InvalidAccessToken = "Mã xác thực không hợp lệ";
            public const string EmailClaimNotFound = "Không tìm thấy xác nhận quyền sở hữu email trong mã thông báo";
            public const string UserNotFoundForEmail = "Không tìm thấy người dùng cho email {0}";
            public const string UserNotFoundForId = "Không tìm thấy người dùng cho id {0}";
            public const string InvalidRefreshTokenForUser = "Mã thông báo làm mới không hợp lệ cho người dùng {0}";
            public const string InvalidInfo = "Thông tin yêu cầu không hợp lệ";
        }
    }
}
