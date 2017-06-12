using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Traffk.Portal.Controllers.Api;

namespace Traffk.Portal.Tests.ControllersTests
{
    [TestClass]
    public class OrchestraControllerTests
    {
        private HttpClient TestClient { get; set; }

        public OrchestraControllerTests()
        {
            TestClient = MockEnvironment.TestClient;
        }

        [TestClass]
        public class GetMethodTests : OrchestraControllerTests
        {

            [TestMethod]
            public void WhenGivenUserWithPermissionGetPlans()
            {
                var accessToken = MockEnvironment.GetBearerToken(TestClient, @"darren@traffk.com", Guid.Parse("58f620c5-0669-44dc-a2fa-d79a3df80103"));

                TestClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                var getPlanResponse = TestClient.GetAsync("/api/v1/Plans").ExecuteSynchronously();
                var json = getPlanResponse.Content.ReadAsStringAsync().ExecuteSynchronously();
                var plans = JsonConvert.DeserializeObject<ICollection<OrchestraController.Plan>>(json);

                Assert.IsNotNull(plans);
                Assert.IsTrue(plans.Any());
                Assert.IsTrue(plans.FirstOrDefault().Name == "Test Plan");

            }

            [Ignore]
            [TestMethod]
            public void WhenGivenUserWithNoPermissionReturn403()
            {
                var accessToken = MockEnvironment.GetBearerToken(TestClient, @"dalfonso@outlook.com", "");

                TestClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                var getPlanResponse = TestClient.GetAsync("/api/v1/Plans").ExecuteSynchronously();

                Assert.IsTrue(getPlanResponse.StatusCode == HttpStatusCode.Forbidden);

                var json = getPlanResponse.Content.ReadAsStringAsync().ExecuteSynchronously();
                var plans = JsonConvert.DeserializeObject<ICollection<OrchestraController.Plan>>(json);

                Assert.IsNull(plans);
            }
        }
    }
}
