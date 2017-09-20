using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevolutionaryStuff.Core;
using Traffk.Portal.Tests.Helpers;
using TraffkPortal;

namespace Traffk.Portal.Tests.ControllersTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestClass]
        public class IndexMethodTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGoingToIndexRedirectToLogin()
            {
                var testFixture = new TestFixture<Startup>();
                var testClient = testFixture.Client;
                testClient.DefaultRequestHeaders.Remove("Authorization");

                var response = testClient.GetAsync("/").ExecuteSynchronously();

                Assert.IsTrue(response.Headers.Location.AbsolutePath == "/Account/Login");
            }
        }
    }
}
