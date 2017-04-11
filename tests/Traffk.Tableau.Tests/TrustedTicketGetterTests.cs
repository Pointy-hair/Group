using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.Tests
{
    [TestClass]
    public class TrustedTicketGetterTests
    {
        //public IOptions<TableauSignInOptions> Options { get; set; }

        //public TrustedTicketGetterTests()
        //{
        //    Options = MockEnvironment.TableauSignInOptions().Object;
        //}

        //[TestClass]
        //public class AuthorizeMethodTests : TrustedTicketGetterTests
        //{
        //    [TestMethod]
        //    public async Task WhenGivenUrlAndUsernameReturnTrustedTicket()
        //    {
        //        var testGetter = new TrustedTicketGetter(Options);
        //        var result = await testGetter.Authorize();
        //        Assert.AreNotEqual("-1", result.Token);
        //    }
        //}
    }
}
