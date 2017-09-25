using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.Tests
{
    [TestClass]
    public class TrustedTicketGetterTests
    {
        public IOptions<TableauSignInOptions> Options { get; set; }

        public TrustedTicketGetterTests()
        {
            Options = MockEnvironment.TableauSignInOptions().Object;
        }

        [TestClass]
        public class AuthorizeMethodTests : TrustedTicketGetterTests
        {
            [TestMethod]
            public void WhenGivenNullCredentialsThrowException()
            {
                Assert.ThrowsException<ArgumentNullException> (()=> new TrustedTicketGetter(Options, null, MockEnvironment.TableauTenantFinder().Object, null));
            }
        }
    }
}
