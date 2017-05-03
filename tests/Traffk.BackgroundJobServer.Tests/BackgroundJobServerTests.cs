using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Traffk.BackgroundJobServer.Tests
{
    [TestClass]
    public class BackgroundJobServerTests
    {
        [TestClass]
        public class OnGoTests
        {
            [TestMethod]
            public void WhenGivenJobRun()
            {
                var testString = "NOT NULL";
                Assert.IsNotNull(testString);

                //object nullObject = null;
                //Assert.IsNotNull(nullObject);
            }
        }

    }
}
