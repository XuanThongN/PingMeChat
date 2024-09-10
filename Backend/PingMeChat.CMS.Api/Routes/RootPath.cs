namespace PingMeChat.CMS.API.Routes
{
    public static class RootPath
    {
        public static class Auth
        {
            public const string LoginRoute = "api/auth/login";
            public const string LogoutRoute = "api/auth/logout";
            public const string RegisterRoute = "api/auth/register";
            public const string ChangePasswordRoute = "api/auth/change-password";
            public const string LockAccountRoute = "api/auth/lock-account/{id}";
            public const string UnLockAccountRoute = "api/auth/unlock-account/{id}";
        }
        public static class Feature
        {
            public static class User
            {
                public const string AddRoute = "api/users/add";
                public const string GetAllRoute = "api/users/get-all";
                public const string GetByIdRoute = "api/users/get-by-id/{id}";
                public const string UpdateRoute = "api/users/update";
                public const string DeleteRoute = "api/users/delete/{id}";
                public const string PaginationRoute = "api/users/pagination";
                public const string GetAllActive = "api/users/get-all-acctive";
            }
            public static class Role
            {
                public const string AddRoute = "api/roles/add";
                public const string GetAllRoute = "api/roles/get-all";
                public const string GetByIdRoute = "api/roles/get-by-id/{id}";
                public const string UpdateRoute = "api/roles/update";
                public const string DeleteRoute = "api/roles/delete/{id}";
                public const string PaginationRoute = "api/roles/pagination";
                public const string GetAllActive = "api/roles/get-all-acctive";
            }
            public static class BidaTable
            {
                public const string AddRoute = "api/bida-tables/add";
                public const string GetAllRoute = "api/bida-tables/get-all";
                public const string GetByIdRoute = "api/bida-tables/get-by-id/{id}";
                public const string UpdateRoute = "api/bida-tables/update";
                public const string DeleteRoute = "api/bida-tables/delete/{id}";
                public const string PaginationRoute = "api/bida-tables/pagination";
                public const string GetEmptiesRoute = "api/bida-tables/get-empties";
                public const string GetTotalWithStatusRoute = "api/bida-tables/get-total-with-status";
            }
            public static class PlayProcess
            {
                public const string StartPlayRoute = "api/play-process/start-play";
                public const string PaymentRoute = "api/play-process/payment";
                public const string DebtRoute = "api/play-process/debt";
                public const string AddProductSessionRoute = "api/play-process/add-product-session";
                public const string ChangeTableRoute = "api/play-process/change-table";
                public const string SplitHourRoute = "api/play-process/split-hour";
                public const string GetAllProductSessionRoute = "api/play-process/get-all-product-session";
            }
            
            public static class Menu
            {
                public const string GetAllByCurrentUserRoute = "api/menus/get-all-by-current-user";
                public const string GetAllTreeRoute = "api/menus/get-all-tree";
                public const string GetControllerByConditionRoute = "api/menus/get-controller-by-condition";
                public const string AddRoute = "api/menus/add";
                public const string GetAllRoute = "api/menus/get-all";
                public const string GetRoleByCurrentUserModuleRoute = "api/menus/get-role-by-current-user-module";
                public const string GetByIdRoute = "api/menus/get-by-id/{id}";
                public const string ChangeStatusRoute = "api/menus/change-status/{id}";
                public const string UpdateRoute = "api/menus/update";
                public const string DeleteRoute = "api/menus/delete/{id}";
            }
        }
    }
}
