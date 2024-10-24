namespace PingMeChat.CMS.API.Routes
{
    public static class ApiRoutes
    {
        public static class Auth
        {
            public const string LoginRoute = "api/auth/login";
            public const string LogoutRoute = "api/auth/logout";
            public const string RegisterRoute = "api/auth/register";
            public const string ChangePasswordRoute = "api/auth/change-password";
            public const string LockAccountRoute = "api/auth/lock-account/{id}";
            public const string UnLockAccountRoute = "api/auth/unlock-account/{id}";
            public const string VerifyCodeRoute = "api/auth/verify-code";
            public const string ResendVerificationCode = "api/auth/resend-verification-code";
            public const string ForgotPasswordRoute = "api/auth/forgot-password";
            public const string VerifyResetCodeRoute = "api/auth/verify-reset-code";
            public const string ResetPasswordRoute = "api/auth/reset-password";

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
                public const string UpdateFCMTokenRoute = "api/users/update-fcm-token";
                public const string UpdateProfileRoute = "api/users/update-profile";
                public const string UpdateAvatarRoute = "api/users/update-avatar";
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

            public static class Group
            {
                public const string PaginationRoute = "api/groups/pagination";
                public const string AddRoute = "api/groups/add";
                public const string GetAllRoute = "api/groups/get-all";
                public const string GetByIdRoute = "api/groups/get-by-id/{id}";
                public const string UpdateRoute = "api/groups/update";
                public const string DeleteRoute = "api/groups/delete/{id}";
                public const string GetAllActive = "api/groups/get-all-acctive";
                public const string ChangeStatusRoute = "api/groups/change-status/{id}";
            }

            // Chat api routes
            public static class Chat
            {
                public const string GetChatListRoute = "api/chats/get-chat-list";
                public const string GetChatDetailRoute = "api/chats/get-chat-detail/{chatId}";
                public const string CreateChatRoute = "api/chats/create-chat";
                public const string UpdateChatRoute = "api/chats/update-chat";
                public const string DeleteChatRoute = "api/chats/delete-chat/{id}";
                public const string AddUsersToChatRoute = "api/chats/add-users-to-chat/{chatId}";
                public const string RemoveUserFromChatRoute = "api/chats/remove-user-from-chat/{chatId}";
            }

            // Message api routes
            public static class Message
            {
                public const string SendMessageRoute = "api/messages/send-message/{chatId}";
                public const string PaginationRoute = "api/messages/pagination";
                public const string GetChatMessagesRoute = "api/messages/{chatId}";
            }

            // Contact api routes
            public static class Contact
            {
                public const string AddRoute = "api/contacts/add";
                public const string GetAllRoute = "api/contacts/get-all";
                public const string GetAllByCurrentUserRoute = "api/contacts/get-all-by-current-user";
                public const string GetByIdRoute = "api/contacts/get-by-id/{id}";
                public const string UpdateRoute = "api/contacts/update";
                public const string DeleteRoute = "api/contacts/delete/{id}";
                public const string PaginationRoute = "api/contacts/pagination";

                public const string AcceptFriendRequestRoute = "api/contacts/accept-friend-request/{contactId}";
                public const string CancelFriendRequestRoute = "api/contacts/cancel-friend-request/{contactId}";
                public const string SendFriendRequestRoute = "api/contacts/send-friend-request";
                public const string RecommendFriendsRoute = "api/contacts/recommend-friends";
                public const string GetAllFriendsStatusRoute = "api/contacts/get-all-friends-status";


            }

            // Attachment api routes
            public static class Attachment
            {
                public const string UploadMultipleFilesRoute = "api/attachments/upload-multiple-files";
                public const string UploadChunkRoute = "api/attachments/upload-chunk";
                public const string CompleteUploadRoute = "api/attachments/complete-upload";
                public const string GetTemporaryFile = "api/attachments/get-temporary-file";
            }

            // Search api routes
            public static class Search
            {
                public const string SearchRoute = "api/search";
            }
        }
    }
}
