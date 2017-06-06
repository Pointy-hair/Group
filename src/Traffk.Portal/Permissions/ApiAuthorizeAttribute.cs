using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using RevolutionaryStuff.Core;
using Traffk.Bal.Permissions;
using TraffkPortal.Permissions;
using TraffkPortal.Services.TenantServices;

namespace Traffk.Portal.Permissions
{
    public class ApiRequirement : IAuthorizationRequirement
    {
        public readonly ApiNames Api;
        public readonly string ClaimType;

        public override string ToString() => $"{this.GetType().Name} api={Api} claimType=[{ClaimType}]";
        public ApiRequirement(ApiNames api)
        {
            Api = api;
            ClaimType = PermissionHelpers.CreateClaimType(api);
        }
    }

    public class ApiHandler : AuthorizationHandler<ApiRequirement>
    {
        private readonly TenantFinderService TenantFinder;

        public ApiHandler(TenantFinderService tenantFinder)
        {
            Requires.NonNull(tenantFinder, nameof(tenantFinder));
            TenantFinder = tenantFinder;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiRequirement requirement)
        {
            var claims = context.User.FindAll(c => c.Type == requirement.ClaimType).ToList();
            if (claims.Count == 0) return Task.FromResult(0);
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

    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly ApiNames Api;

        public ApiAuthorizeAttribute(ApiNames api)
            : base(PortalPermissionHelpers.CreatePolicyName(api))
        {
            Api = api;
        }
    }
}
