using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.App.Repositories;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Indentity.RefreshTokens;
using PingMeChat.CMS.Application.Feature.Service.ErrorLog;
using PingMeChat.CMS.Application.Feature.Service.Groups;
using PingMeChat.CMS.Application.Feature.Service.Menus;
using PingMeChat.CMS.Application.Feature.Service.MvcControllerDiscovery;
using PingMeChat.CMS.Application.Feature.Service.Roles;
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
                    .AddSingleton<IMvcControllerDiscoveryService, MvcControllerDiscoveryService>()
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
                    .AddScoped<IUserSessionRepository, UserSessionRepository>();
        }
    }
}