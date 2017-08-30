using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Permissions
{
    public static class PermissionHelpers
    {
        public static IdentityRoleClaim<string> CreateIdentityRoleClaim(PermissionNames permission)
            => new IdentityRoleClaim<string>
            {
                ClaimType = CreateClaimType(permission),
                ClaimValue = new PermissionClaimValue(true).ToJson()
            };

        public static string CreateClaimType(PermissionNames permissionName)
            => $"{TraffkHelpers.TraffkUrn}/claims/{permissionName}";

        public static string CreateClaimType(ApiNames apiName)
        {
            var apiRoot = $"{TraffkHelpers.TraffkUrn}/claims/apis";

            if (apiName == ApiNames.Base)
            {
                return apiRoot;
            }

            return $"{apiRoot}/{apiName}";
        }
    }
}
