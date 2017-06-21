using Microsoft.AspNetCore.Http;
using RevolutionaryStuff.Core;
using System;
using System.IdentityModel.Tokens.Jwt;
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
                    var currentTenantId = traffkTenantFinder.GetTenantIdAsync().ExecuteSynchronously();
                    var tenantIdClaim = bearerTokenIdentity.Claims.First(x => x.Properties.Values.Contains(JwtRegisteredClaimNames.Sub));
                    var tenantIdInToken = int.Parse(tenantIdClaim.Value);

                    if (tenantIdInToken == currentTenantId)
                    {
                        await Next(context);
                        return;
                    }

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                //No bearer token in use, move on
                await Next(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Authentication error.");
                return;
            }
        }
    }
}
