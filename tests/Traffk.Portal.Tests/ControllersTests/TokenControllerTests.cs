using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Traffk.Portal.Tests.Helpers;
using TraffkPortal;

namespace Traffk.Portal.Tests.ControllersTests
{
    [TestClass]
    public class TokenControllerTests
    {
        private readonly HttpClient TestClient;

        public TokenControllerTests()
        {
            var testFixture = new TestFixture<Startup>();
            TestClient = testFixture.Client;
        }

        [TestClass]
        public class GetTokenMethodTests : TokenControllerTests
        {
            private class TokenResponse
            {
                public string access_token { get; set; }
            }

            [Ignore]
            [TestMethod]
            public void WhenGivenUsernameAndPasswordGetBearerToken()
            {
                var testFormContent = new Dictionary<string, string>
                {
                    ["username"] = @"darren@traffk.com",
                    ["password"] = @""
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
