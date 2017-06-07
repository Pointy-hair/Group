using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevolutionaryStuff.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ILogger Logger; 

        public TokenController(IOptions<TokenProviderOptions> options,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
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
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        public async Task<TokenResponse> GetToken()
        {
            List<Claim> claims = null;
            var username = HttpContext.Request.Form["username"];
            var password = HttpContext.Request.Form["password"];
            var apiKey = HttpContext.Request.Form["apiKey"];

            if (!String.IsNullOrEmpty(password))
            {
                var userClaims = await GetClaims(username, password);
                claims = userClaims.ToList();
            }

            if (!String.IsNullOrEmpty(apiKey))
            {
                var userClaims = await GetClaims(username, Guid.Parse(apiKey));
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

        private async Task<IEnumerable<Claim>> GetClaims(string username, string password)
        {
            var user = await UserManager.FindByNameAsync(username);
            var result = await SignInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var claimsPrincipal = UserClaimsPrincipalFactory.CreateAsync(user).ExecuteSynchronously();

                return claimsPrincipal.Claims;
            }

            return null;
        }

        private async Task<IEnumerable<Claim>> GetClaims(string username, Guid apiKey)
        {
            var user = await UserManager.FindByNameAsync(username);
            if (user.Settings.ApiKey.Equals(apiKey))
            {
                await SignInManager.SignInAsync(user, false);
                var claimsPrincipal = UserClaimsPrincipalFactory.CreateAsync(user).ExecuteSynchronously();
                return claimsPrincipal.Claims;
            }

            return null;
        }
    }
}