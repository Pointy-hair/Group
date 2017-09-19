using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using Traffk.Portal.Tests.Helpers;
using TraffkPortal;

namespace Traffk.Portal.Tests
{
    public static class MockEnvironment
    {
        private static readonly string TokenAuthRoute = "/api/token/authenticate";
        private static HttpClient HttpClient = null;
        private static string AccessToken_p = null;

        public static HttpClient TestClientWithBearerToken
        {
            get
            {
                if (HttpClient == null)
                {
                    var testFixture = new TestFixture<Startup>();
                    HttpClient = testFixture.Client;
                    var accessToken = GetBearerToken(HttpClient, @"darren@traffk.com", Guid.Parse("58f620c5-0669-44dc-a2fa-d79a3df80103"));
                    HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
                }
                return HttpClient;
            }
        }

        public static HttpClient TestClient
        {
            get
            {
                if (HttpClient == null)
                {
                    var testFixture = new TestFixture<Startup>();
                    HttpClient = testFixture.Client;
                }
                return HttpClient;
            }
        }

        public static string AccessToken
        {
            get { return AccessToken_p; }
        }


        public static string GetBearerToken(HttpClient client, string username, string password)
        {
            if (String.IsNullOrEmpty(AccessToken_p))
            {
                var tokenRequestFormContent = new Dictionary<string, string>
                {
                    ["username"] = username,
                    ["password"] = password
                };
                var content = new FormUrlEncodedContent(tokenRequestFormContent);

                var response = client.PostAsync(TokenAuthRoute, content).ExecuteSynchronously();

                var json = response.Content.ReadAsStringAsync().ExecuteSynchronously();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

                AccessToken_p = tokenResponse.access_token;
            }

            return AccessToken_p;
        }

        public static string GetBearerToken(HttpClient client, string username, Guid apiKey)
        {
            if (String.IsNullOrEmpty(AccessToken_p))
            {
                var tokenRequestFormContent = new Dictionary<string, string>
                {
                    ["username"] = username,
                    ["apiKey"] = apiKey.ToString()
                };
                var content = new FormUrlEncodedContent(tokenRequestFormContent);

                var response = client.PostAsync(TokenAuthRoute, content).ExecuteSynchronously();

                var json = response.Content.ReadAsStringAsync().ExecuteSynchronously();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

                AccessToken_p = tokenResponse.access_token;
            }

            return AccessToken_p;
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
    }
}
