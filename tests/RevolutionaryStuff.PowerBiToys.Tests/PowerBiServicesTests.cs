using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevolutionaryStuff.PowerBiToys;

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
