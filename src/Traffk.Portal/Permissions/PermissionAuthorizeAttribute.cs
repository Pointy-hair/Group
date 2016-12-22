using Microsoft.AspNetCore.Authorization;
using RevolutionaryStuff.Core;
using System.Linq;
using System.Threading.Tasks;
using TraffkPortal.Services.TenantServices;
using Traffk.Bal.Permissions;

namespace TraffkPortal.Permissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public readonly PermissionNames Permission;
        public readonly string ClaimType;

        public override string ToString() => $"{this.GetType().Name} permission={Permission} claimType=[{ClaimType}]";
        public PermissionRequirement(PermissionNames permission)
        {
            Permission = permission;
            ClaimType = PermissionHelpers.CreateClaimType(permission);
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly TenantFinderService TenantFinder;

        public PermissionHandler(TenantFinderService tenantFinder)
        {
            Requires.NonNull(tenantFinder, nameof(tenantFinder));
            TenantFinder = tenantFinder;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var claims = context.User.FindAll(c => c.Type == requirement.ClaimType).ToList();
            if (claims.Count==0) return Task.FromResult(0);
            foreach (var c in claims)
            {
                var pcv = PermissionClaimValue.CreateFromJson(c.Value);
                if (pcv.Granted)
                {
                    context.Succeed(requirement);
                    break;
                }
            }
            return Task.FromResult(0);
        }
    }

    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly PermissionNames Permission;

        public PermissionAuthorizeAttribute(PermissionNames permission)
            : base(PortalPermissionHelpers.CreatePolicyName(permission))
        {
            Permission = permission;
        }
    }
}
