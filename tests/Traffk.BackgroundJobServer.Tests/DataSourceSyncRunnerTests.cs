using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Traffk.BackgroundJobServer.Tests
{
    [TestClass]
    public class DataSourceSyncRunnerTests
    {
        [TestClass]
        public class ConstructorTests
        {
            [TestMethod]
            public void WhenGivenNullsThrowException()
            {
                Assert.ThrowsException<NullReferenceException>(() => new DataSourceSyncRunner(null, null, null, null, null, null, null, null));
            }
        }
    }
}
