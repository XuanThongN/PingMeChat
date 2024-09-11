using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Interfaces;
using PingMeChat.CMS.Entities.Module;
using PingMeChat.CMS.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore
{
    public class AppDBContext : DbContext
    {
        #region basic
        DbSet<Account> Accounts { get; set; }
        DbSet<Role> Roles { get; set; }
        public DbSet<UserMenu> UserMenus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<GroupRole> GroupRoles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        DbSet<Notification> Notifications { get; set; }

        DbSet<Media> Media { get; set; }
        #endregion

        // feature
        DbSet<Attachment> Attachments { get; set; }
        DbSet<Call> Calls { get; set; }
        DbSet<CallParticipant> CallParticipants { get; set; }
        DbSet<Chat> Chats { get; set; }
        DbSet<Contact> Contacts { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<MessageStatus> MessageStatus { get; set; }
        DbSet<UserChat> UserChats { get; set; }


        public AppDBContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            AppModuleBuider.RegisterModule(modelBuilder);
            IsDeletedFilter(modelBuilder);

            modelBuilder.HasPostgresExtension("citext");

            modelBuilder.Entity<Menu>().HasData(
             new Menu
             {
                 Id = "31feb02e-9c05-4930-a914-0af953707dfd",
                 Title = "Bảng điều khiển",
                 Icon = "fa fa-home",
                 Url = "/home",
                 ParentId = null,
                 MenuType = false,
                 SortOrder = 1,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "fa6f5f76-2266-4f57-8962-258fc43619dd",
                 Title = "Danh mục",
                 Icon = "fas fa-list-ul",
                 Url = "",
                 ParentId = null,
                 MenuType = true,
                 SortOrder = 2,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "928c2a06-0b95-45b2-8850-24452b13d82a",
                 Title = "Loại bàn bida",
                 Icon = "fas fa-table",
                 Url = "/bidatabletype",
                 ParentId = "fa6f5f76-2266-4f57-8962-258fc43619dd",
                 MenuType = false,
                 SortOrder = 3,
                 Access = null,

                 IsActive = true
             },
             new Menu
             {
                 Id = "5ae1d36a-df86-4ebf-a51f-1b33922aa633",
                 Title = "Bàn bida",
                 Icon = "fas fa-bowling-ball",
                 Url = "/bidatable",
                 ParentId = "fa6f5f76-2266-4f57-8962-258fc43619dd",
                 MenuType = false,
                 SortOrder = 4,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "cfcfa81f-53f9-4382-82a3-d753aa88be3f",
                 Title = "Bán hàng",
                 Icon = "fas fa-shopping-cart",
                 Url = "",
                 ParentId = null,
                 MenuType = true,
                 SortOrder = 5,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "384c16a4-d389-4fea-b554-e702a333cf60",
                 Title = "Hóa đơn",
                 Icon = "fab fa-first-order-alt",
                 Url = "/order",
                 ParentId = "cfcfa81f-53f9-4382-82a3-d753aa88be3f",
                 MenuType = false,
                 SortOrder = 6,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "bad6c610-681c-47d4-aec0-6551aa308485",
                 Title = "Đối tác",
                 Icon = "fas fa-people-arrows",
                 Url = "",
                 ParentId = null,
                 MenuType = true,
                 SortOrder = 7,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "336ec1d7-c59b-4c83-a919-53d29b6c003a",
                 Title = "Khách hàng",
                 Icon = "fas fa-user-friends",
                 Url = "/customer",
                 ParentId = "bad6c610-681c-47d4-aec0-6551aa308485",
                 MenuType = false,
                 SortOrder = 8,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "ac8f8e58-6e25-4126-aec8-b491e71155da",
                 Title = "Sản phẩm, dịch vụ",
                 Icon = "fab fa-servicestack",
                 Url = "",
                 ParentId = null,
                 MenuType = true,
                 SortOrder = 9,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "db942e9c-d34f-4be1-ad5d-a4e1b105f8b9",
                 Title = "Sản phẩm",
                 Icon = "fab fa-product-hunt",
                 Url = "/product",
                 ParentId = "ac8f8e58-6e25-4126-aec8-b491e71155da",
                 MenuType = false,
                 SortOrder = 10,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "f0e8c848-8f6d-4a7b-9b8e-7c7e9f11a7f9",
                 Title = "Quản lý kho",
                 Icon = "fas fa-box",
                 Url = "",
                 ParentId = null,
                 MenuType = true,
                 SortOrder = 11,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "a4e8a6f2-9c3d-4e2b-8f1a-5c5e6b7d8e9f",
                 Title = "Tồn kho",
                 Icon = "fas fa-warehouse",
                 Url = "/inventory",
                 ParentId = "f0e8c848-8f6d-4a7b-9b8e-7c7e9f11a7f9",
                 MenuType = false,
                 SortOrder = 12,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "b2c7d8e9-f0a1-4b2c-3d4e-5f6g7h8i9j0k",
                 Title = "Nhập kho",
                 Icon = "fas fa-truck-loading",
                 Url = "/inventoryimport",
                 ParentId = "f0e8c848-8f6d-4a7b-9b8e-7c7e9f11a7f9",
                 MenuType = false,
                 SortOrder = 13,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "c3d4e5f6-g7h8-4i9j-0k1l-2m3n4o5p6q7r",
                 Title = "Xuất kho",
                 Icon = "fas fa-truck",
                 Url = "/inventoryexport",
                 ParentId = "f0e8c848-8f6d-4a7b-9b8e-7c7e9f11a7f9",
                 MenuType = false,
                 SortOrder = 14,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                 Title = "Báo cáo",
                 Icon = "fas fa-chart-bar",
                 Url = "/report",
                 ParentId = null,
                 MenuType = false,
                 SortOrder = 15,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                 Title = "Quản trị",
                 Icon = "fa fa-id-card",
                 Url = "",
                 ParentId = null,
                 MenuType = true,
                 SortOrder = 16,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                 Title = "Tài khoản",
                 Icon = "fas fa-user",
                 Url = "/user",
                 ParentId = "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                 MenuType = false,
                 SortOrder = 17,
                 Access = null,
                 IsActive = true
             },
             new Menu
             {
                 Id = "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                 Title = "Phân quyền",
                 Icon = "fas fa-user-tag",
                 Url = "/role",
                 ParentId = "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                 MenuType = false,
                 SortOrder = 18,
                 Access = null,
                 IsActive = true
             }
 );

        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Where(e =>
                e.Entity is AuditableBaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((AuditableBaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((AuditableBaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }
            return base.SaveChanges();
        }
        private void IsDeletedFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IAuditableBaseEntity).IsAssignableFrom(entityType.ClrType) && entityType.BaseType == null)
                {
                    var parameter = Expression.Parameter(entityType.ClrType);
                    var propertyMethodInfo = typeof(EF).GetMethod("Property")?.MakeGenericMethod(typeof(bool));
                    var isDeletedProperty =
                        Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDeleted"));
                    var constantFalse = Expression.Constant(false);
                    var equalExpression = Expression.Equal(isDeletedProperty, constantFalse);
                    var lambda = Expression.Lambda(equalExpression, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}
