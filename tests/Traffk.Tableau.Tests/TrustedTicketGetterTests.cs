using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Traffk.Tableau.Tests
{
    [TestClass]
    public class TrustedTicketGetterTests
    {
        [TestClass]
        public class AuthorizeMethodTests
        {
            [Ignore]
            [TestMethod]
            public async Task WhenGivenUrlAndUsernameReturnTrustedTicket()
            {
                var testGetter = new TrustedTicketGetter();
                var result = await testGetter.Authorize();
                Assert.AreNotEqual("-1", result.Token);
            }
        }
    }
}
