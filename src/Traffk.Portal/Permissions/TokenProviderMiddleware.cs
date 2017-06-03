using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Traffk.Portal.Permissions
{
    /// <summary>
    /// 6-2-17 not used yet
    /// </summary>

    public class TokenProviderMiddleware
    {
        //private readonly RequestDelegate Next;
        //private readonly TokenProviderOptions Options;
        //private readonly JsonSerializerSettings SerializerSettings;

        //public TokenProviderMiddleware(
        //    RequestDelegate next,
        //    IOptions<TokenProviderOptions> options)
        //{
        //    Next = next;

        //    Options = options.Value;

        //    SerializerSettings = new JsonSerializerSettings
        //    {
        //        Formatting = Formatting.Indented
        //    };
        //}

        //public Task Invoke(HttpContext context)
        //{
        //    // If the request path doesn't match, skip
        //    if (!context.Request.Path.Equals(Options.Path, StringComparison.Ordinal))
        //    {
        //        return Next(context);
        //    }

        //    // Request must be POST with Content-Type: application/x-www-form-urlencoded
        //    if (!context.Request.Method.Equals("POST")
        //       || !context.Request.HasFormContentType)
        //    {
        //        context.Response.StatusCode = 400;
        //        return context.Response.WriteAsync("Bad request.");
        //    }


        //    return GenerateToken(context);
        //}

        //private async Task GenerateToken(HttpContext context)
        //{
        //    var username = context.Request.Form["username"];
        //    var password = context.Request.Form["password"];

        //    var identity = await Options.IdentityResolver(username, password);
        //    if (identity == null)
        //    {
        //        context.Response.StatusCode = 400;
        //        await context.Response.WriteAsync("Invalid username or password.");
        //        return;
        //    }

        //    var now = DateTime.UtcNow;

        //    // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
        //    // You can add other claims here, if you want:
        //    var claims = new Claim[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, username),
        //        new Claim(JwtRegisteredClaimNames.Jti, await Options.NonceGenerator()),
        //        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        //    };

        //    // Create the JWT and write it to a string
        //    var jwt = new JwtSecurityToken(
        //        issuer: Options.Issuer,
        //        audience: Options.Audience,
        //        claims: claims,
        //        notBefore: now,
        //        expires: now.Add(Options.Expiration),
        //        signingCredentials: Options.SigningCredentials);
        //    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        //    var response = new
        //    {
        //        access_token = encodedJwt,
        //        expires_in = (int)Options.Expiration.TotalSeconds
        //    };

        //    // Serialize and return the response
        //    context.Response.ContentType = "application/json";
        //    await context.Response.WriteAsync(JsonConvert.SerializeObject(response, SerializerSettings));
        //}

    }
}
