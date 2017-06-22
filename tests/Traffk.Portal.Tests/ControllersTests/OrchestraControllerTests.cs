using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Traffk.Orchestra.Models;
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
                var testClient = MockEnvironment.TestClient;

                var getPlanResponse = testClient.GetAsync("/api/v1/Plans").ExecuteSynchronously();
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

            [TestMethod]
            public void WhenGivenDrugAlternativeQueryReturnValidResponse()
            {
                var testClient = MockEnvironment.TestClient;

                var response = testClient.GetAsync("api/v1/Drugs/59746017710/Alternative/30/30").ExecuteSynchronously();
                var json = response.Content.ReadAsStringAsync().ExecuteSynchronously();
                var drugAlternativeResponse = JsonConvert.DeserializeObject<DrugOption[]>(json);

                Assert.IsNotNull(drugAlternativeResponse);
                Assert.AreEqual("Cyclobenzaprine HCl", drugAlternativeResponse[0].ChemicalName);
            }
        }
    }
}
