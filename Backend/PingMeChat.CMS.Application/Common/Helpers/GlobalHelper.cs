using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.App.Repositories;
using PingMeChat.CMS.Application.Feature.Indentity.Auth;
using PingMeChat.CMS.Application.Feature.Indentity.RefreshTokens;
using PingMeChat.CMS.Application.Feature.Service;
using PingMeChat.CMS.Application.Feature.Service.BidaTables;
using PingMeChat.CMS.Application.Feature.Service.BidaTableSessionServicess;
using PingMeChat.CMS.Application.Feature.Service.Customers;
using PingMeChat.CMS.Application.Feature.Service.ErrorLog;
using PingMeChat.CMS.Application.Feature.Service.Groups;
using PingMeChat.CMS.Application.Feature.Service.InventoryExports;
using PingMeChat.CMS.Application.Feature.Service.InventoryImports;
using PingMeChat.CMS.Application.Feature.Service.Inventorys;
using PingMeChat.CMS.Application.Feature.Service.Menus;
using PingMeChat.CMS.Application.Feature.Service.MvcControllerDiscovery;
using PingMeChat.CMS.Application.Feature.Service.OrderCancelHistorys;
using PingMeChat.CMS.Application.Feature.Service.Orders;
using PingMeChat.CMS.Application.Feature.Service.Products;
using PingMeChat.CMS.Application.Feature.Service.Roles;
using PingMeChat.CMS.Application.Feature.Service.SessionServices;
using PingMeChat.CMS.Application.Feature.Service.SessionServicess;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace PingMeChat.CMS.Application.Common.Config
{
    public static class GlobalHelper
    {
        public static void RegisterAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BidaTableMapper));
            services.AddAutoMapper(typeof(BidaTableTypeMapper));
            services.AddAutoMapper(typeof(BidaTableSessionMapper));
            services.AddAutoMapper(typeof(ServiceSessionServiceMapper));
            services.AddAutoMapper(typeof(AccountMapper));
            services.AddAutoMapper(typeof(OrderMapper));
            services.AddAutoMapper(typeof(OrderHistoryMapper));
            services.AddAutoMapper(typeof(ProductMapper));
            services.AddAutoMapper(typeof(InventoryMapper));
            services.AddAutoMapper(typeof(CustomerMapper));
            services.AddAutoMapper(typeof(UserMapper));
            services.AddAutoMapper(typeof(RoleMapper));
            services.AddAutoMapper(typeof(InventoryImportMapper));
            services.AddAutoMapper(typeof(TokenMapper));
            services.AddAutoMapper(typeof(MenuMapper));
            services.AddAutoMapper(typeof(GroupMapper));
        }
        public static void RegisterServiceLifetimer(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>()
                    .AddScoped<ITokenService, TokenService>()
                    .AddScoped<IBidaTableService, BidaTableService>()
                    .AddScoped<IBidaTableTypeService, BidaTableTypeService>()
                    .AddScoped<IServiceSessionService, ServiceSessionService>()
                    .AddScoped<IBidaTableSessionServices, BidaTableSessionServices>()
                    .AddScoped<IProductService, ProductService>()
                    .AddScoped<IOrderService, OrderService>()
                    .AddScoped<IOrderHistoryService, OrderHistoryService>()
                    .AddScoped<ICustomerService, CustomerService>()
                    .AddScoped<IUserService, UserService>()
                    .AddScoped<IRoleService, RoleService>()
                    .AddScoped<IMenuService, MenuService>()
                    .AddScoped<IInventoryService, InventoryService>()
                    .AddScoped<IInventoryImportService, InventoryImportService>()
                    .AddScoped<IInventoryExportService, InventoryExportService>()
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
                    .AddScoped<IBidaTableSessionRepository, BidaTableSessionRepository>()
                    .AddScoped<IBidaTableRepository, BidaTableRepository>()
                    .AddScoped<IBidaTableTypeRepository, BidaTableTypeRepository>()
                    .AddScoped<IServiceSessionRepository, ServiceSessionRepository>()
                    .AddScoped<IProductRepository, ProductRepository>()
                    .AddScoped<INotificationRepository, NotificationRepository>()
                    .AddScoped<IRoleRepository, RoleRepository>()
                    .AddScoped<IMediaRepository, MediaRepository>()
                    .AddScoped<IOrderRepository, OrderRepository>()
                    .AddScoped<IOrderHistoryRepository, OrderHistoryRepository>()
                    .AddScoped<IRoleRepository, RoleRepository>()
                    .AddScoped<IMenuRepository, MenuRepository>()
                    .AddScoped<IRolesMenuRepository, RolesMenuRepository>()
                    .AddScoped<IUsersMenuRepository, UsersMenuRepository>()
                    .AddScoped<IInventoryRepository, InventoryRepository>()
                    .AddScoped<IInventoryImportRepository, InventoryImportRepository>()
                    .AddScoped<IInventoryExportRepository, InventoryExportRepository>()
                    .AddScoped<IGroupsRepository, GroupsRepository>()
                    .AddScoped<ILogErrorRepository, LogErrorRepository>()
                    .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                    .AddScoped<IUserSessionRepository, UserSessionRepository>()
                    .AddScoped<ICustomerRepository, CustomerRepository>();
        }
    }
}