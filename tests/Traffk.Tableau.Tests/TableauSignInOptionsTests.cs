using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.Tests
{
    [TestClass]
    public class TableauSignInOptionsTests
    {
        [TestClass]
        public class ConstructorTests
        {
            [TestMethod]
            public void WhenGivenNonHttpsUrlThrowException()
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TableauSignInOptions("http://www.test.com"));
            }
        }
    }
}
