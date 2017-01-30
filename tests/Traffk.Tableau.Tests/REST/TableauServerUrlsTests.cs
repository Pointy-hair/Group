using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Tableau.REST;

namespace Traffk.Tableau.Tests.REST
{
    [TestClass]
    public class TableauServerUrlsTests
    {
        [TestClass]
        public class ConstructorTests
        {
            [TestMethod]
            public void WhenGivenUrlReturnTableauServerUrlsInstance()
            {
                var testObject =
                    TableauServerUrls.FromContentUrl("http://traffk-dev-tab.eastus.cloudapp.azure.com/#/projects", 1);
                Assert.IsNotNull(testObject);
            }
        }
    }
}
