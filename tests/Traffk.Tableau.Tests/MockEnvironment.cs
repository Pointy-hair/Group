using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.Tests
{
    public static class MockEnvironment
    {
        public static Mock<IOptions<TableauSignInOptions>> TableauSignInOptions()
        {
            var optionsMock = new Mock<IOptions<TableauSignInOptions>>();
            optionsMock.Setup(x => x.Value)
                .Returns(new TableauSignInOptions("https://tableau-dev.traffk.com",
                    "Darren Alfonso", "DarrenTraffkTableau"));

            return optionsMock;
        }

        public static Mock<IOptions<TableauSignInOptions>> TableauSignInOptions(string url, string username, string password)
        {
            var optionsMock = new Mock<IOptions<TableauSignInOptions>>();
            optionsMock.Setup(x => x.Value)
                .Returns(new TableauSignInOptions(url,
                    username, password));

            return optionsMock;
        }
    }
}
