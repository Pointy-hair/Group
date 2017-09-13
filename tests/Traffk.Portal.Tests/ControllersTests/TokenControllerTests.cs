using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Traffk.Portal.Tests.Helpers;
using TraffkPortal;

namespace Traffk.Portal.Tests.ControllersTests
{
    [TestClass]
    public class TokenControllerTests
    {
        private HttpClient TestClient { get; set; }

        public TokenControllerTests()
        {
            TestClient = MockEnvironment.TestClient;
        }

        [TestClass]
        public class GetTokenMethodTests : TokenControllerTests
        {
            private class TokenResponse
            {
                public string access_token { get; set; }
            }

            [TestMethod]
            public void WhenGivenUsernameAndIncorrectApiKeyDoNotGetBearerToken()
            {
                TestClient.DefaultRequestHeaders.Remove("Authorization");

                var testFormContent = new Dictionary<string, string>
                {
                    ["username"] = @"darren@traffk.com",
                    ["apiKey"] = @"48f620c5-0669-44dc-a2fa-d79a3df80103"
                };
                var content = new FormUrlEncodedContent(testFormContent);

                var response = TestClient.PostAsync("/api/token/authenticate", content).ExecuteSynchronously();

                var json = response.Content.ReadAsStringAsync().ExecuteSynchronously();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

                Assert.IsNotNull(response);
                Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
                Assert.AreEqual("Authentication error.", tokenResponse.access_token);
            }

            [TestMethod]
            public void WhenGivenUsernameAndApiKeyGetBearerToken()
            {
                TestClient.DefaultRequestHeaders.Remove("Authorization");

                var testFormContent = new Dictionary<string, string>
                {
                    ["username"] = @"darren@traffk.com",
                    ["apiKey"] = @"58f620c5-0669-44dc-a2fa-d79a3df80103"
                };
                var content = new FormUrlEncodedContent(testFormContent);

                var response = TestClient.PostAsync("/api/token/authenticate", content).ExecuteSynchronously();

                var json = response.Content.ReadAsStringAsync().ExecuteSynchronously();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);

                Assert.IsNotNull(response);
                Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
                Assert.IsFalse(String.IsNullOrEmpty(tokenResponse.access_token));
            }
        }
    }
}
