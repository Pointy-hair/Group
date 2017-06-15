using Microsoft.AspNetCore.Http;
using RevolutionaryStuff.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal;

namespace Traffk.Portal.Permissions
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate Next;
        
        public TokenAuthenticationMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task Invoke(HttpContext context, ITraffkTenantFinder traffkTenantFinder)
        {
            try
            {
                var authenticateInfo = await context.Authentication.GetAuthenticateInfoAsync("Bearer");
                var bearerTokenIdentity = authenticateInfo?.Principal;

                if (bearerTokenIdentity != null)
                {
                    var tenantIdClaim = bearerTokenIdentity.Claims.First(x => x.Type == "TenantId");
                    var tenantIdInToken = int.Parse(tenantIdClaim.Value);
                    var currentTenantId = traffkTenantFinder.GetTenantIdAsync().ExecuteSynchronously();

                    if (tenantIdInToken == currentTenantId)
                    {
                        await Next(context);
                    }

                    context.Response.StatusCode = 400;
                    return;
                }

                //No bearer token in use, move on
                await Next(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Authentication error.");
                return;
            }
        }
    }
}
