using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevolutionaryStuff.Core;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Traffk.Bal.Data.Rdb;
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
        public TokenResponse GetToken()
        {
            var claims =
                GetClaims(HttpContext.Request.Form["username"], HttpContext.Request.Form["password"])
                    .ExecuteSynchronously().ToList();

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Options.NonceGenerator().ExecuteSynchronously()));

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

        private Task<IEnumerable<Claim>> GetClaims(string username, string password)
        {
            var user = UserManager.FindByNameAsync(username).ExecuteSynchronously();
            var result =
                SignInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false)
                    .ExecuteSynchronously();

            if (result.Succeeded)
            {
                var claimsPrincipal = UserClaimsPrincipalFactory.CreateAsync(user).ExecuteSynchronously();

                return Task.FromResult(claimsPrincipal.Claims);
            }

            return Task.FromResult<IEnumerable<Claim>>(null);
        }
    }
}