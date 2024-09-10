using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PingMeChat.CMS.AdminPage.Permissions
{
    internal class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        Task<AuthorizationPolicy> IAuthorizationPolicyProvider.GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();


        Task<AuthorizationPolicy?> IAuthorizationPolicyProvider.GetFallbackPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
        Task<AuthorizationPolicy?> IAuthorizationPolicyProvider.GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return Task.FromResult(policy.Build());
            }
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
