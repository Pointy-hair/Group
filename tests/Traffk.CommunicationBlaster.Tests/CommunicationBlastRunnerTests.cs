using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Traffk.CommunicationBlaster.Tests
{
    [TestClass]
    public class CommunicationBlastRunnerTests
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
