using PingMeChat.CMS.Application;
using PingMeChat.CMS.Application.Common.Config;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Middlewares;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using PingMeChat.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using PingMeChat.CMS.Application.Feature.ChatHubs;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using PingMeChat.CMS.Application.Feature.Services.RabbitMQServices;

var builder = WebApplication.CreateBuilder(args);

Settings.Configuration = builder.Configuration;
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
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<AuthorizationFilter>();
//});


var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<AppDBContext>(o =>
    o.UseNpgsql(connectionString, b => b.EnableRetryOnFailure()));

// Add services to the container.
#region authen
builder.Services
    .AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
{
    options.SaveToken = false; // Không lưu token - nó là option để việc tạo mới acesstoken khi nó hết hạn - token mới tạo ra ứng dụng sẽ biết
    options.RequireHttpsMetadata = false; // Nên đặt thành true trong môi trường production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // Thêm kiểm tra thời hạn token
        ValidateIssuerSigningKey = true, // Thêm kiểm tra khóa ký
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ClockSkew = TimeSpan.Zero // Loại bỏ thời gian dung sai mặc định (5 phút)

    };
    // Xử lý sự kiện khi xác thực thất bại
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            // Luôn đọc token từ header, bỏ qua cache
            //context.Token = context.Request.Headers["Authorization"]
            //    .FirstOrDefault()?.Split(" ").Last();
            //return Task.CompletedTask;

            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var refreshToken = context.Request.Headers["RefreshToken"].FirstOrDefault();

            var path = context.HttpContext.Request.Path;
            if (path.StartsWithSegments("/chatHub"))
            {
                // Ưu tiên lấy token từ query string cho SignalR
                accessToken = context.Request.Query["access_token"].FirstOrDefault() ?? accessToken;
                refreshToken = context.Request.Query["refresh_token"].FirstOrDefault() ?? refreshToken;
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                context.HttpContext.Items["RefreshToken"] = refreshToken;
            }

            return Task.CompletedTask;
        }
    };
});
//// Thêm phân quyền sau xác thực
//builder.Services.AddAuthorization();
#endregion

#region Controller

builder.Services.AddControllers(op => { })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
        options.InvalidModelStateResponseFactory = CustomValidator.MakeValidationResponse;
    })
    .ConfigureApiBehaviorOptions(op => { op.SuppressModelStateInvalidFilter = true; })
    .AddJsonOptions(op =>
    {
        op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        op.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

#endregion

#region RabbitMQServices
builder.Services.AddHostedService<MessageProcessingService>();
builder.Services.AddScoped<MessageProcessor>();
#endregion

#region Swagger Api
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.AddSignalRSwaggerGen();
        options.EnableAnnotations();
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "PingMeChat API", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
        options.SchemaFilter<EnumSchemaFilter>();
    });

builder.Services.AddSwaggerGenNewtonsoftSupport();
#endregion

#region register SignalR
builder.Services.AddSignalR();
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region registerService
builder.Services.AddScoped<UserExistsFilter>();
builder.Services.AddScoped<InfoUserNotFoundFilter>();
builder.Services.AddScoped<ModelStateFilter>();
builder.Services.AddScoped<ValidateUserAndModelFilter>();
GlobalHelper.RegisterAutoMapper(builder.Services);
GlobalHelper.RegisterServiceLifetimer(builder.Services);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();  // Thêm dịch vụ bộ nhớ cache
builder.Services.AddHttpClient();
#endregion 

#region Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton(cloud =>
{
    var config = cloud.GetService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});
#endregion

// Initialize Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-adminsdk.json"))
});

var app = builder.Build();

#region add middlware
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();  // Thêm middleware xử lý rate limiting
    //.UseMiddleware<TokenRefreshMiddleware>();
#endregion
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
}

app.UseHttpsRedirection();
// app.UseCors(options => options
//         .WithOrigins("http://localhost:9000", "https://localhost:9000")
//        .AllowAnyMethod()
//        .AllowAnyHeader()
//        .AllowCredentials()
//    );
app.UseCors(options => options
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    );
//app.UseCookiePolicy(new CookiePolicyOptions
//{
//    MinimumSameSitePolicy = SameSiteMode.None,
//    Secure = CookieSecurePolicy.Always
//});
app.UseMiddleware<TokenRefreshMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.Run();
