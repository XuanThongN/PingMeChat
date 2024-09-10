using Microsoft.AspNetCore.Authorization;

namespace PingMeChat.CMS.AdminPage.Permissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; private set; }
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
