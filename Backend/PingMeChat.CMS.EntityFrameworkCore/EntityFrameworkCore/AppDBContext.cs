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

            #region Seeding Menu data
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
             #endregion
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
