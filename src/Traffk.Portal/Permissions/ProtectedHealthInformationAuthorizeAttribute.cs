using Microsoft.AspNetCore.Authorization;
using RevolutionaryStuff.Core;
using System.Threading.Tasks;
using Traffk.Bal.Email;
using Traffk.Bal.Permissions;
using TraffkPortal.Services;
using TraffkPortal.Services.TenantServices;

namespace TraffkPortal.Permissions
{
    public class ProtectedHealthInformationRequirement : PermissionRequirement
    {
        public ProtectedHealthInformationRequirement()
            : base(PermissionNames.ProtectedHealthInformation)
        { }
    }

    public class ProtectedHealthInformationAuthorizeAttribute : AuthorizeAttribute
    {
        public ProtectedHealthInformationAuthorizeAttribute()
            : base(PortalPermissionHelpers.ExplicitPolicyNames.ProtectedHealthInformation)
        { }
    }

    public class ProtectedHealthInformationHandler : PermissionHandler
    {
        protected readonly CurrentContextServices Current;

        public ProtectedHealthInformationHandler(CurrentContextServices current, TenantFinderService tenantFinder)
            : base(tenantFinder)
        {
            Current = current;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (requirement is ProtectedHealthInformationRequirement)
            {
                base.HandleRequirementAsync(context, requirement).ExecuteSynchronously();
                if (context.HasSucceeded)
                {
                    if (MailHelpers.IsEmailAddressPartOfHostname(Current.User.Email, Current.Tenant.TenantSettings.ProtectedHealthInformationViewableByEmailAddressHostnames))
                    {
                        goto Done;
                    }
                    context.Fail();
                }
            }
            Done:
            return Task.FromResult(0);
        }
    }
}
