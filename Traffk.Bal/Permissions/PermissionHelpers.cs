using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Traffk.Bal.Permissions
{
    public static class PermissionHelpers
    {
        public static IdentityRoleClaim<string> CreateIdentityRoleClaim(PermissionNames permission)
        {
            return new IdentityRoleClaim<string>
            {
                ClaimType = CreateClaimType(permission),
                ClaimValue = new PermissionClaimValue(true).ToJson()
            };
        }

        public static string CreateClaimType(PermissionNames permissionName)
        {
            return $"{TraffkHelpers.TraffkUrn}/claims/{permissionName}";
        }
    }
}
