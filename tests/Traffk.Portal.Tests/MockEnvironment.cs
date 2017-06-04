using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;

namespace Traffk.Portal.Tests
{
    public static class MockEnvironment
    {
        public static string GetBearerToken(HttpClient client, string username, string password)
        {
            var tokenRequestFormContent = new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password
            };
            var content = new FormUrlEncodedContent(tokenRequestFormContent);

            var response = client.PostAsync("/api/token", content).ExecuteSynchronously();

            var json = response.Content.ReadAsStringAsync().ExecuteSynchronously();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

            return tokenResponse.access_token;
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
    }
}
