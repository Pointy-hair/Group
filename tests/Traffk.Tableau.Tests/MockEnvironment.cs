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
                .Returns(new TableauSignInOptions("https://tableau-dev.traffk.com"));

            return optionsMock;
        }

        public static Mock<IOptions<TableauSignInOptions>> TableauSignInOptions(string url)
        {
            var optionsMock = new Mock<IOptions<TableauSignInOptions>>();
            optionsMock.Setup(x => x.Value)
                .Returns(new TableauSignInOptions(url));

            return optionsMock;
        }

        public static Mock<ITableauUserCredentials> TableauAdminCredentials()
        {
            var mock = new Mock<ITableauUserCredentials>();
            mock.Setup(x => x.UserName).Returns("Darren Alfonso");
            mock.Setup(x => x.Password).Returns("DarrenTraffkTableau");
            return mock;
        }

        public static Mock<ITableauUserCredentials> TableauUserCredentials()
        {
            var mock = new Mock<ITableauUserCredentials>();
            mock.Setup(x => x.UserName).Returns("Test");
            mock.Setup(x => x.Password).Returns("TraffkTestTableau");
            return mock;
        }

        public static Mock<ITableauUserCredentials> TableauUserCredentials(string username, string password)
        {
            var mock = new Mock<ITableauUserCredentials>();
            mock.Setup(x => x.UserName).Returns(username);
            mock.Setup(x => x.Password).Returns(password);
            return mock;
        }

        public static Mock<ITableauTenantFinder> TableauTenantFinder()
        {
            var tenantFinderMock = new Mock<ITableauTenantFinder>();
            tenantFinderMock.Setup(x => x.GetTenantIdAsync()).Returns(Task.FromResult<string>(null));
            return tenantFinderMock;
        }
    }
}
