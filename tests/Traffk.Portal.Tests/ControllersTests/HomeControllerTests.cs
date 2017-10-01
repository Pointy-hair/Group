using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevolutionaryStuff.Core;
using System;
using TraffkPortal.Controllers;

namespace Traffk.Portal.Tests.ControllersTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestClass]
        public class ConstructorTests
        {
            [TestMethod]
            public void WhenGivenNullDbContextThrowException()
            {
                Assert.ThrowsException<ArgumentNullException>(() => new HomeController(null, null, MockEnvironment.CreateTestLogger()));
            }

        }

        [TestClass]
        public class IndexMethodTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGoingToIndexRedirectToLogin()
            {
                var testClient = MockEnvironment.TestClient;
                testClient.DefaultRequestHeaders.Remove("Authorization");

                var response = testClient.GetAsync("/").ExecuteSynchronously();

                Assert.IsTrue(response.Headers.Location.AbsolutePath == "/Account/Login");
            }
        }
    }
}
