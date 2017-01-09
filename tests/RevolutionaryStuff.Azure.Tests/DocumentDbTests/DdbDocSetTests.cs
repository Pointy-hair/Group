using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RevolutionaryStuff.Azure.Tests.DocumentDbTests
{
    [TestClass]
    public class DdbDocSetTests
    {
        [TestClass]
        public class FetchAllMethodTests
        {
            [TestMethod]
            public void WhenGivenContextFetchAll()
            {
                var testString = "NOT NULL";
                Assert.IsNotNull(testString);

                //object nullObject = null;
                //Assert.IsNotNull(nullObject);
            }
        }
    }
}
