using PingMeChat.CMS.AdminPage.Common.Filter;
using PingMeChat.CMS.AdminPage.Common.Middleware;
using PingMeChat.CMS.AdminPage.Permissions;
using PingMeChat.CMS.Application.Common.Config;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using PingMeChat.Shared;
using PingMeChat.Shared.Authorization;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//Get key value from file appsetting.json
Settings.Configuration = builder.Configuration;
// Đọc cấu hình từ appsettings.json hoặc các nguồn cấu hình khác
var configuration = builder.Configuration;
#region logger
// Thiết lập Serilog từ cấu hình
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    //.Enrich.FromLogContext()
    .CreateLogger();

// Cấu hình logging cho ứng dụng
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration);  // Đọc từ cấu hình
});
#endregion

// Cấu hình bộ lọc kiểm tra quyền truy cập của người dùng
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthorizationFilter>();
});

builder.Services.AddSignalR();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddControllersWithViews().AddNToastNotifyNoty(new NotyOptions
{
    ProgressBar = true,
    Timeout = 3000,
});

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDBContext>(o =>
    o.UseNpgsql(connectionString, b => b.EnableRetryOnFailure()));

//builder.Services.AddIdentity<Account, Role>(op =>
//{
//    op.User.RequireUniqueEmail = true;

//    op.Password.RequireDigit = true;
//    op.Password.RequireLowercase = true;
//    op.Password.RequireNonAlphanumeric = false;
//    op.Password.RequireUppercase = false;
//    op.Password.RequiredLength = 6;
//    op.Password.RequiredUniqueChars = 1;
//})
//    .AddEntityFrameworkStores<AppDBContext>()
//    .AddDefaultTokenProviders();
#region Authentication

builder.Services
    .AddAuthorization(options =>
    {
        // cấu hình chính sách cho các trường hợp phức tạp
        // các trường hợp bình thường thì mình có thể cấu hình như bên dưới
        options.AddPolicy(PermissionNames.Admin.Page_Home, policy =>
        {
            //  người dùng cần phải thuộc ít nhất một trong các vai trò được liệt kê ở đây
            policy.RequireRole(RoleNames.Staff, RoleNames.IT_Manager, RoleNames.Super_Admin, RoleNames.Manager);
        });
        options.AddPolicy(PermissionNames.Admin.Page_FormRegister, policy =>
        {
            //  người dùng cần phải thuộc ít nhất một trong các vai trò được liệt kê ở đây
            policy.RequireRole(RoleNames.IT_Manager, RoleNames.Super_Admin, RoleNames.Manager);
        }
        );
        options.AddPolicy(PermissionNames.Admin.Page_Role, policy =>
        {
            policy.RequireRole(RoleNames.Super_Admin);
            /* policy.RequireAssertion(
                 context => context.User.HasClaim(claim =>
                 (claim.Type == "permissions" &&
                 (claim.Value == ClaimNames.All))));*/ // Xem và thêm nhóm quyền cho người dùng
        });
        options.AddPolicy(PermissionNames.Admin.Page_Role_Show, policy =>
        {
            policy.RequireRole(RoleNames.IT_Manager, RoleNames.Super_Admin);
            policy.RequireAssertion(
                context => context.User.HasClaim(claim =>
                (claim.Type == "permissions" &&
                (claim.Value == ClaimNames.Show)))); // Xem và thêm nhóm quyền cho người dùng
        });
        options.AddPolicy(PermissionNames.Admin.Page_Role_Add_For_User, policy => // nhớ muốn edut thì phải xem được
        {
            policy.RequireRole(RoleNames.IT_Manager, RoleNames.Super_Admin);
            policy.RequireAssertion(
                context => context.User.HasClaim(claim =>
                (claim.Type == "permissions" &&
                (claim.Value == ClaimNames.Edit)))); // Xem và thêm nhóm quyền cho người dùng
        });
    })
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
   // định nghĩa cơ chế cho token
   // Nếu người dùng chưa được xác thực ( hoặc hết hạn). Nó sẽ được điều hướng tới trang đăng nhập được cấu hình bên dưới
   // thời gian tồn tại của token đó sẽ là 30 ngày
   // và tên cookie đó sẽ là UserLoginCookie được lưu trên máy người dùng
   // Mỗi lần gửi yêu cầu nó sẽ gán cookie đó lên header


   // [Authorize] :
   // 1. Nếu có 1 yêu cầu mà đk cần có [Authorize]. Điều đầu tiên nó dùng middleware UseAuthentication được sử dụng để xác thực yêu cầu
   // 2. middleware sẽ kiểm tra cookie để coi người dùng đã đăng nhập hay chưa
   // 3. Nếu cookie đã tồn tại và chưa hết hạn thì middleware sẽ dùng cookie để yêu cầu
   // 4. Nếu chưa tồn tại hay hết hạn. Nó sẽ yêu cầu người dùng đăng nhập 

   // JwtBearerDefaults.AuthenticationScheme: Tên yêu cầu để xác thực. Ưng đặt sao cũng được
   // UserLoginCookie tên cookie lưu trên máy người dùng
   // Mỗi khi yêu cầu đăng nhập thành công, phương thức AddCookie sẽ tạo ra 1 cookie để lưu thông tin người dùng
    .AddCookie(JwtBearerDefaults.AuthenticationScheme, config =>
    {
        config.Cookie.HttpOnly = true;
        config.Cookie.Name = "AdminLoginCookie";
        config.LoginPath = "/Account/Login";
        config.ExpireTimeSpan = System.TimeSpan.FromMinutes(int.Parse(builder.Configuration["JWT:TokenExpiresMinutes"]));
        config.AccessDeniedPath = $"/PageError/Error401";
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
});

#endregion;
// phiên làm việc của client ( trình duyệt)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// config hangfire
builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
// config email
//builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<UserExistsFilter>();
builder.Services.AddScoped<InfoUserNotFoundFilter>();
builder.Services.AddScoped<ModelStateFilter>();
builder.Services.AddScoped<ValidateUserAndModelFilter>();
GlobalHelper.RegisterServiceLifetimer(builder.Services);
GlobalHelper.RegisterAutoMapper(builder.Services);
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();


var app = builder.Build();

#region add middlware
app.UseMiddleware<ExceptionMiddleware>()
    //.UseMiddleware<TokenRefreshMiddleware>()
    .UseMiddleware<ResponseMiddleware>();
#endregion

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection()
.UseStaticFiles()
.UseRouting()
.UseAuthentication()
.UseAuthorization()
.UseSession()
.UseHttpsRedirection()
.UseCookiePolicy();
//.UseHangfireDashboard("/hangfire", new DashboardOptions
//{
//    //AppPath = "" //The path for the Back To Site link. Set to null in order to hide the Back To  Site link.
//    DashboardTitle = "My Website",
//    Authorization = new[]
//        {
//                new HangfireCustomBasicAuthenticationFilter{
//                    User = builder.Configuration["HangfireSettings:UserName"],
//                    Pass =  builder.Configuration["HangfireSettings:Password"]
//                }
//            }
//})
//.UseHangfireServer();

//  sẽ được gọi mỗi khi có một yêu cầu HTTP đến server
// được xử lý trước khi các middleware khác được gọi.
//app.Use(async (context, next) =>
//   {
//       var jwToken = context.Session.GetString("AccessToken");
//       if (!string.IsNullOrEmpty(jwToken))
//       {
//           context.Request.Headers.Add("Authorization", "Bearer " + jwToken);
//       }
//       await next();
//   });
//app.MapControllerRoute(
//    name: "PDFRoute",
//    pattern: "RegistrationForm/ShowFilePDF/{id}/{formTypeId}",
//    defaults: new { controller = "RegistrationForm", action = "ShowFilePDF" });
//app.MapControllerRoute(
//    name: "ConfirmCancelRoute",
//    pattern: "RegistrationForm/ConfirmCancel/{id}/{formTypeId}",
//    defaults: new { controller = "RegistrationForm", action = "ConfirmCancel" });
app.MapControllerRoute(
       name: "default",
       pattern: "{controller=Home}/{action=Index}/{id?}");

/*app.MapC*/

// nơi lưu trữ các thông báo
//app.MapHub<HubContainer>("/hub-container");
app.UseNToastNotify();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<AppDBContext>();
//        context.Database.Migrate();
//        Log.Information("Database migrated successfully.");
//    }
//    catch (Exception ex)
//    {
//        Log.Error(ex, "An error occurred while migrating the database.");
//    }
//}
app.Run();
