using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.App.Repositories;
using PingMeChat.CMS.Application.Feature.ChatHubs;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Indentity.RefreshTokens;
using PingMeChat.CMS.Application.Feature.Service.Attachments;
using PingMeChat.CMS.Application.Feature.Service.CallParticipants;
using PingMeChat.CMS.Application.Feature.Service.Calls;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Contacts;
using PingMeChat.CMS.Application.Feature.Service.ErrorLogs;
using PingMeChat.CMS.Application.Feature.Service.Groups;
using PingMeChat.CMS.Application.Feature.Service.Menus;
using PingMeChat.CMS.Application.Feature.Service.Messages;
using PingMeChat.CMS.Application.Feature.Service.MessageStatuses;
using PingMeChat.CMS.Application.Feature.Service.MvcControllerDiscovery;
using PingMeChat.CMS.Application.Feature.Service.Notifications;
using PingMeChat.CMS.Application.Feature.Service.Roles;
using PingMeChat.CMS.Application.Feature.Service.UserChats;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.Common.Config
{
    public static class GlobalHelper
    {
        public static void RegisterAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AccountMapper));
            services.AddAutoMapper(typeof(UserMapper));
            services.AddAutoMapper(typeof(RoleMapper));
            services.AddAutoMapper(typeof(TokenMapper));
            services.AddAutoMapper(typeof(MenuMapper));
            services.AddAutoMapper(typeof(GroupMapper));
            services.AddAutoMapper(typeof(ContactMapper));
        }
        public static void RegisterServiceLifetimer(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>()
                    .AddScoped<ITokenService, TokenService>()
                    .AddScoped<IUserService, UserService>()
                    .AddScoped<IRoleService, RoleService>()
                    .AddScoped<IMenuService, MenuService>()
                    .AddScoped<IErrorLogService, ErrorLogService>()
                    .AddScoped<IGroupService, GroupService>()
                    .AddScoped<IAttachmentService, AttachmentService>()
                    .AddScoped<ICallParticipantService, CallParticipantService>()
                    .AddScoped<IUserChatService, UserChatService>()
                    .AddScoped<ICallService, CallService>()
                    .AddScoped<IChatService, ChatService>()
                    .AddScoped<IContactService, ContactService>()
                    .AddScoped<IMessageService, MessageService>()
                    .AddScoped<IMessageStatusService, MessageStatusService>()
                    .AddScoped<INotificationService, NotificationService>()
                    .AddSingleton<IMvcControllerDiscoveryService, MvcControllerDiscoveryService>()
                    .AddScoped<IChatHubService, ChatHubService>() // Thêm dịch vụ ChatHubService
                    .AddSingleton<IUserConnectionManager, UserConnectionManager>()
                    .AddSingleton<IUserIdProvider, CustomUserIdProvider>()
                    // Dang ky Dich vu FCM
                    .AddScoped<IFCMService, FCMService>()
                    .AddSingleton<IUriService>(o =>
                    {
                        var accessor = o.GetRequiredService<IHttpContextAccessor>();
                        var request = accessor.HttpContext.Request;
                        var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                        return new UriService(uri);
                    })

                    .AddScoped<IJwtLib, JwtLib>()
                    .AddScoped<IUnitOfWork, UnitOfWork>()
                    .AddScoped<IDbContextTransaction>(_ => null)
                    .AddScoped<IRoleRepository, RoleRepository>()
                    .AddScoped<IGroupRoleRepository, GroupRoleRepository>()
                    .AddScoped<IGroupUserRepository, GroupUserRepository>()
                    .AddScoped<IAccountRepository, AccountReponsitory>()
                    .AddScoped<INotificationRepository, NotificationRepository>()
                    .AddScoped<IRoleRepository, RoleRepository>()
                    .AddScoped<IMediaRepository, MediaRepository>()
                    .AddScoped<IRoleRepository, RoleRepository>()
                    .AddScoped<IMenuRepository, MenuRepository>()
                    .AddScoped<IRolesMenuRepository, RolesMenuRepository>()
                    .AddScoped<IUsersMenuRepository, UsersMenuRepository>()
                    .AddScoped<IGroupsRepository, GroupsRepository>()
                    .AddScoped<ILogErrorRepository, LogErrorRepository>()
                    .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                    .AddScoped<IUserSessionRepository, UserSessionRepository>()
                    .AddScoped<IUserChatRepository, UserChatRepository>()
                    .AddScoped<ICallRepository, CallRepository>()
                    .AddScoped<ICallParticipantRepository, CallParticipantRepository>()
                    .AddScoped<IChatRepository, ChatRepository>()
                    .AddScoped<IContactRepository, ContactRepository>()
                    .AddScoped<IMessageRepository, MessageRepository>()
                    .AddScoped<IMessageStatusRepository, MessageStatusRepository>()
                    .AddScoped<INotificationRepository, NotificationRepository>()
                    .AddScoped<IAttachmentRepository, AttachmentRepository>()
                    ;
        }
    }
}