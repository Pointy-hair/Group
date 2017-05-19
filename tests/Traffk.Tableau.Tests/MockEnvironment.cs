using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using RevolutionaryStuff.Core;
using Serilog;
using ILogger = Serilog.ILogger;
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

        public static ILogger CreateTestLogger()
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.AzureTableStorageWithProperties("DefaultEndpointsProtocol=https;AccountName=devhiptraffk;AccountKey=3AJ7i+qzBaq+WkkUNcAyTlhx1pRrfGg6ZiqZGLZj3Ga7j2cyzwjCpfe7dr2mtIQwn/jDrCf0xxVWrJEGh9l9Yg==;",
                        storageTableName: "AppLogsDev")
                    .CreateLogger();
        }
    }
}
