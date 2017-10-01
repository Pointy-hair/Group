using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Traffk.BackgroundJobServer.Tests
{
    [TestClass]
    public class TenantedJobRunnerTests
    {
        [TestClass]
        public class ConstructorTests
        {
            [TestMethod]
            public void WhenGivenNullsThrowException()
            {
                Assert.ThrowsException<NullReferenceException>(() => new TenantedJobRunner(null, null, null, null, null, null, null, null, null));
            }
        }
    }
}
