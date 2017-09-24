using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Traffk.Bal.Permissions;

namespace Traffk.Bal.Data.Rdb.TraffkTenantModel
{
    public partial class ApplicationRole : IdentityRole, ITraffkTenanted
    {

        public List<RoleClaim> Claims { get; set; }

        public bool HasPermission(PermissionNames permission)
        {
            var claimType = PermissionHelpers.CreateClaimType(permission);
            foreach (var claim in this.Claims)
            {
                if (claim.ClaimType == claimType) return true;
            }
            return false;
        }

        public static ApplicationRole Create(string name, PermissionNames[] permissions = null, int? tenantId = null)
        {
            var r = new ApplicationRole
            {
                Name = name,
                NormalizedName = name.ToLower(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            if (permissions != null)
            {
                foreach (var permission in permissions.Distinct())
                {
                    r.Claims.Add(PermissionHelpers.CreateIdentityRoleClaim(permission));
                }
            }
            if (tenantId != null)
            {
                r.TenantId = tenantId.Value;
            }
            return r;
        }

        public static ApplicationRole CreateConfigurationMasterRole(int? tenantId = null)
        {
            return ApplicationRole.Create(
                "ConfigurationMaster",
                new[] { PermissionNames.ManageTenants, PermissionNames.ManageUsers, PermissionNames.ManageRoles },
                tenantId);
        }
    }
}
