using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared.Authorization
{
    public static class RoleNames
    {
        public const string Super_Admin = "SuperAdmin"; // khi cấp cho 1 tài khoản có quyền này. Nhớ luôn tạo full claim cho nó
        public const string IT_Manager = "ITMANAGER";
        public const string Teacher = "TEACHER";
        public const string Student = "STUDENT";
        public const string Manager = "Manager";
        public const string Staff = "Staff";
    }
}
