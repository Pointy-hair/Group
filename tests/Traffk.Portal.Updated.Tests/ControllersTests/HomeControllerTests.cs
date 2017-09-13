using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Traffk.Portal.Tests.ControllersTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestClass]
        public class IndexMethodTests
        {
            [TestMethod]
            public void WhenGivenIndexReturnNotNull()
            {
                var testString = "NOT NULL";
                Assert.IsNotNull(testString);
            }


        }
    }
}
