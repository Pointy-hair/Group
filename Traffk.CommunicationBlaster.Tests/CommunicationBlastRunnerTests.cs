using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.CommunicationBlaster;

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
