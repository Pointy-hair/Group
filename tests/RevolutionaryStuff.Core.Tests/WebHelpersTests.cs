using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RevolutionaryStuff.Core;

namespace RevolutionaryStuff.Core.Tests
{
    [TestClass]
    public class WebHelpersTests
    {
        [TestClass]
        public class CreateHttpContentMethodTests
        {
            [TestMethod]
            public void WhenGivenStringCreateHttpContent()
            {
                var testString = "NOT NULL";
                Assert.IsNotNull(testString);

                //object nullObject = null;
                //Assert.IsNotNull(nullObject);
            }
        }
    }
}
