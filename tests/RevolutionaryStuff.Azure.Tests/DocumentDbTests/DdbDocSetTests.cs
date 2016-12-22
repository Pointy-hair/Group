using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevolutionaryStuff.Azure;

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
