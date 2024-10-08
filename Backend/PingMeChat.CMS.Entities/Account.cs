using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.Shared.Enum;

namespace PingMeChat.CMS.Entities.Users
{
    //public partial class Account : IdentityUser, IAuditableBaseEntity
    //{
    //    public Account()
    //    {
    //        UsersMenus = new HashSet<UsersMenus>();
    //    }
    //    public override string Id { get; set; } = Guid.NewGuid().ToString();
    //    public override string? UserName { get; set; }
    //    public DateTime? DateRegistered { get; set; } = DateTime.Now;
    //    public bool IsLocked { get; set; }
    //    public string? FullName { get; set; }
    //    // token
    //    public string? AccessToken { get; set; }
    //    public string? RefreshToken { get; set; }
    //    public DateTime? RefreshTokenExpiryTime { get; set; }
    //}

    public class Account : AuditableBaseEntity
    {
        public Account()
        {
            GroupUsers = new HashSet<GroupUser>();
            UserMenus = new HashSet<UserMenu>();
            UserSessions = new HashSet<UserSession>();
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsLocked { get; set; } // khóa tài khoản
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; } // ngày sinh
        // Thêm giới tính
        public bool Gender { get; set; } // giới tính true: Nam, false: Nữ

        public string? AvatarUrl { get; set; }
        public string? FCMToken { get; set; } // token FCM

        public string? VerificationCode { get; set; } // mã xác thực
        public DateTime? CodeExpiryTime { get; set; } // thời gian hết hạn mã xác thực
        public bool IsVerified { get; set; } = false; // đã xác thực, mặc định là chưa xác thực

        public string? ResetPasswordCode { get; set; } // mã reset password
        public DateTime? ResetPasswordCodeExpiryTime { get; set; } // thời gian hết hạn mã reset password

        public virtual ICollection<GroupUser> GroupUsers { get; set; }
        public virtual ICollection<UserMenu> UserMenus { get; set; }
        public virtual ICollection<UserSession> UserSessions { get; set; }


    }

    
}
