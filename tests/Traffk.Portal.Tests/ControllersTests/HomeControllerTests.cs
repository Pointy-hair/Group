using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevolutionaryStuff.Core;

namespace Traffk.Portal.Tests.ControllersTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestClass]
        public class IndexMethodTests
        {
            [TestMethod]
            public void WhenGoingToIndexRedirectToLogin()
            {
                var testClient = MockEnvironment.TestClient;

                var response = testClient.GetAsync("/").ExecuteSynchronously();

                Assert.IsTrue(response.StatusCode == HttpStatusCode.Found);
                Assert.IsTrue(response.Headers.Location.AbsolutePath == "/Account/Login");
            }

            [TestMethod]
            public void WhenGoingToPrivacyPolicyAndTermsAllowAnonymousUser()
            {
                var testClient = MockEnvironment.TestClient;

                var response = testClient.GetAsync("/Terms").ExecuteSynchronously();


            }
        }
    }
}
