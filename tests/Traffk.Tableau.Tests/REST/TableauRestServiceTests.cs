using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;
using Moq;

namespace Traffk.Tableau.Tests.REST
{
    [TestClass]
    public class TableauRestServiceTests
    {
        public IOptions<TableauSignInOptions> Options { get; set; }

        public TableauRestServiceTests()
        {
            var optionsMock = new Mock<IOptions<TableauSignInOptions>>();
            Options = optionsMock.Object;
            optionsMock.Setup(x => x.Value)
                .Returns(new TableauSignInOptions("http://traffk-dev-tab.eastus.cloudapp.azure.com/#/projects",
                    "Test", "TraffkTestTableau"));
        }

        [TestClass]
        public class SignInMethodTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenGivenUrlSignIn()
            {
                var testService = new TableauRestService(Options);
                Assert.IsNotNull(testService);
            }
        }

        [TestClass]
        public class DownloadProjectsListMethodTests
        {
            [TestMethod]
            public void WhenSignedInDownloadAllProjects()
            {
                var testService = new TableauRestService();
                var testUrls = TableauServerUrls.FromContentUrl("http://traffk-dev-tab.eastus.cloudapp.azure.com/#/projects", 1);
                var signIn = testService.SignIn(testUrls, "Test", "TraffkTestTableau");
                var projects = testService.DownloadProjectsList(testUrls, signIn);
                Assert.IsNotNull(projects);
                Assert.IsTrue(projects.Projects.Any());
            }
        }

        [TestClass]
        public class DownloadViewsFromSiteMethodTests
        {
            [TestMethod]
            public void WhenSignedInDownloadAllViews()
            {
                var testService = new TableauRestService();
                var testUrls = TableauServerUrls.FromContentUrl("http://traffk-dev-tab.eastus.cloudapp.azure.com/#/projects/1/", 50);
                var signIn = testService.SignIn(testUrls, "Test", "TraffkTestTableau");
                var views = testService.DownloadViewsForSite(testUrls, signIn);
                Assert.IsNotNull(views);
                Assert.IsTrue(views.Views.Any());
            }
        }
    }
}
