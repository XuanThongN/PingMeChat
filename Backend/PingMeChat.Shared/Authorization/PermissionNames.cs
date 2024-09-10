using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared.Authorization
{
    public static class PermissionNames
    {
        public static class Admin
        {
            public const string Page_Home = "Page.Home";
            public const string Page_Role = "Page.Role";
            public const string Page_Role_Show = "Page.Role.Show"; // chỉ xem
            public const string Page_Role_Add_For_User = "Page.Role.Add.For.User"; //  thêm role cho người dùng
            public const string Page_Account = "Page.Account";
            public const string Page_Staff = "Page.Staff";
            public const string Page_Student = "Page.Student";
            public const string Page_FormRegister = "Page.FormRegister";
        }
        public static class Client
        {
            public const string Page_FormRegister = "Page.FormRegister";
        }
    }
}
