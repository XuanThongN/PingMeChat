using Microsoft.AspNetCore.Identity;
using PingMeChat.CMS.Entities;
using PingMeChat.Shared.Enum;
using PingMeChat.CMS.Entities.Users;

namespace PingMeChat.CMS.AdminPage.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<Account> userManager, RoleManager<Role> roleManager)
        {
           // await roleManager.CreateAsync(new Role(RoleType.SUPPERADMIN.ToString()));

        }
    }
}
