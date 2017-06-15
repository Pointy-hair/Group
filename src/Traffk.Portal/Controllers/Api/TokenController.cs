using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevolutionaryStuff.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Traffk.Bal;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using Traffk.Portal.Models.ApiModels;
using Traffk.Portal.Permissions;

namespace Traffk.Portal.Controllers.Api
{
    [Authorize]
    [ApiAuthorize(ApiNames.Base)]
    [Produces("application/json")]
    [Route("api/token")]
    public class TokenController : BaseApiController
    {
        private readonly TokenProviderOptions Options;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> UserClaimsPrincipalFactory;
        private readonly SigningCredentials SigningCredentials;
        private readonly ITraffkTenantFinder TraffkTenantFinder;

        public TokenController(IOptions<TokenProviderOptions> options,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            ITraffkTenantFinder traffkTenantFinder,
            ILogger logger) : base(logger)
        {
            Options = options.Value;

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Options.SecretKey));
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            Requires.NonNull(Options.Issuer, nameof(Options.Issuer));
            Requires.NonNull(Options.Audience, nameof(Options.Audience));
            Requires.True(Options.Expiration > TimeSpan.Zero, nameof(Options.Expiration));

            UserManager = userManager;
            SignInManager = signInManager;
            UserClaimsPrincipalFactory = userClaimsPrincipalFactory;
            TraffkTenantFinder = traffkTenantFinder;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        public async Task<TokenResponse> GetTokenAsync()
        {
            List<Claim> claims = null;
            var username = HttpContext.Request.Form["username"];
            var apiKey = HttpContext.Request.Form["apiKey"];

            if (!String.IsNullOrEmpty(apiKey))
            {
                var userClaims = await GetClaimsAsync(username, apiKey);
                claims = userClaims.ToList();
            }

            claims?.Add(new Claim(JwtRegisteredClaimNames.Jti, Options.NonceGenerator().ExecuteSynchronously()));

            var jwt = new JwtSecurityToken(
                issuer: Options.Issuer,
                audience: Options.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.Add(Options.Expiration),
                signingCredentials: SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new TokenResponse
            {
                access_token = encodedJwt,
                expires_in = (int)Options.Expiration.TotalSeconds,
                resource = Options.Audience,
            };

            Log("Retrieved token");

            return response;
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(string username, string apiKey)
        {
            var user = await UserManager.FindByNameAsync(username);

            if (!String.IsNullOrEmpty(user.Settings.ApiKey) && !String.IsNullOrEmpty(apiKey) && user.Settings.ApiKey.Equals(apiKey))
            {
                await SignInManager.SignInAsync(user, false);
                var claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);

                var tenantId = await TraffkTenantFinder.GetTenantIdAsync();
                var tenantIdClaim = new Claim("TenantId", tenantId.ToString());
                var claims = claimsPrincipal.Claims.ToList();
                claims.Add(tenantIdClaim);

                return claims;
            }

            return null;
        }
    }
}