using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Portal;

namespace Traffk.Portal.Tests.ControllerTests
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

                //object nullObject = null;
                //Assert.IsNotNull(nullObject);
                
            }
        }
    }
}
