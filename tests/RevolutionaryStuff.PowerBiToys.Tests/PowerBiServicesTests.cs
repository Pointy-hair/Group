using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RevolutionaryStuff.PowerBiToys.Tests
{
    [TestClass]
    public class PowerBiServicesTests
    {
        [TestClass]
        public class GetMethodTests
        {
            [TestMethod]
            public void WhenGivenUriGetPowerBiResult()
            {
                var testString = "NOT NULL";
                Assert.IsNotNull(testString);

                //object nullObject = null;
                //Assert.IsNotNull(nullObject);
            }
        }
    }
}
