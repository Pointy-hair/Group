using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Tableau.REST;

namespace Traffk.Tableau.Tests.REST
{
    [TestClass]
    public class TableauRestServiceTests
    {
        [TestClass]
        public class SignInMethodTests
        {
            [TestMethod]
            public void WhenGivenUrlSignIn()
            {
                var testService = new TableauRestService();
                var testUrls = TableauServerUrls.FromContentUrl("http://traffk-dev-tab.eastus.cloudapp.azure.com/#/projects", 1);
                var signIn = testService.SignIn(testUrls, "Test", "TraffkTestTableau");
                Assert.IsNotNull(signIn);
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
                var signIn = testService.SignIn(testUrls, "Darren Alfonso", "DAmico2936!");
                var projects = testService.DownloadProjectsList(signIn, testUrls);
                Assert.IsNotNull(projects);
                Assert.IsTrue(projects.Projects.Any());
            }
        }
    }
}
