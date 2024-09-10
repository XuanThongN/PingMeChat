using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Entities.Module;
using PingMeChat.CMS.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json;

namespace PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore
{
    public class AppModuleBuider
    {
        public static void RegisterModule(ModelBuilder builder)
        {
            #region main

            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(x => x.Id);

            });


            builder.Entity<Account>(e =>
            {
                e.HasMany(x => x.UserMenus)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Account>(e =>
            {
                e.HasMany(x => x.GroupUsers)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Account>(e =>
            {
                e.HasMany(x => x.UserSessions)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Account>(e =>
            {
                e.HasMany(x => x.UserSessions)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Group>()
              .HasMany(x => x.GroupUsers)
              .WithOne(x => x.Group)
              .HasForeignKey(x => x.GroupId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Group>()
                .HasMany(x => x.GroupRoles)
                .WithOne(x => x.Group)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Menu>()
                .HasMany(x => x.RoleMenus)
                .WithOne(x => x.Menu)
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Menu>()
                .HasMany(x => x.UserMenus)
                .WithOne(x => x.Menu)
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Role>()
                .HasMany(x => x.RoleMenus)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Role>()
                .HasMany(x => x.GroupRoles)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RefreshToken>()
              .HasMany(x => x.UserSessions)
              .WithOne(x => x.RefreshToken)
               .HasForeignKey(x => x.RefreshTokenId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<GroupRole>()
                .HasKey(k => new
                {
                    k.GroupId,
                    k.RoleId
                });

            builder.Entity<RoleMenu>()
                .HasKey(k => new
                {
                    k.RoleId,
                    k.MenuId
                });

            builder.Entity<UserMenu>()
                .HasKey(k => new
                {
                    k.AccountId,
                    k.MenuId
                });

            builder.Entity<GroupUser>()
                .HasKey(k => new
                {
                    k.GroupId,
                    k.AccountId
                });

            #endregion main

            builder.Entity<Order>(e =>
            {
                e.Property(o => o.OrderDetails)
                    .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions)null) : null,
                        v => !string.IsNullOrEmpty(v) ?
                            JsonSerializer.Deserialize<List<OrderDetail>>(v, (JsonSerializerOptions)null) : null
                    )
                    .HasColumnType("nvarchar(max)");
            });

            builder.Entity<OrderHistory>(e =>
            {
                e.Property(o => o.OrderDetails)
                    .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions)null) : null,
                        v => !string.IsNullOrEmpty(v) ?
                            JsonSerializer.Deserialize<List<OrderDetail>>(v, (JsonSerializerOptions)null) : null
                    )
                    .HasColumnType("nvarchar(max)");
            });

            builder.Entity<InventoryImport>(e =>
            {
                e.Property(o => o.InventoryDetailsImports)
                    .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions)null) : null,
                        v => !string.IsNullOrEmpty(v) ?
                            JsonSerializer.Deserialize<List<InventoryDetailsImport>>(v, (JsonSerializerOptions)null) : null
                    )
                    .HasColumnType("nvarchar(max)");
            });

            builder.Entity<InventoryExport>(e =>
            {
                e.Property(o => o.InventoryDetailsExports)
                    .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions)null) : null,
                        v => !string.IsNullOrEmpty(v) ?
                            JsonSerializer.Deserialize<List<InventoryDetailsExport>>(v, (JsonSerializerOptions)null) : null
                    )
                    .HasColumnType("nvarchar(max)");
            });
            
            builder.Entity<Menu>(e =>
            {
                e.Property(o => o.Access)
                    .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, (JsonSerializerOptions)null) : null,
                        v => !string.IsNullOrEmpty(v) ?
                            JsonSerializer.Deserialize<List<MvcActionInfo>>(v, (JsonSerializerOptions)null) : null
                    )
                    .HasColumnType("nvarchar(max)");
            });
        }
    }
}
