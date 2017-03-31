using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features.Authentication;
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
            Options = MockEnvironment.TableauSignInOptions().Object;
        }

        [TestClass]
        public class CreateSiteMethodTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenTenantNameCreateSite()
            {
                var testService = new TableauRestService(new TrustedTicketGetter(Options), Options);
                string url;
                var site = ((ITableauRestService)testService).CreateSite("Test Tenant", out url);
                Assert.IsNotNull(site);
                StringAssert.Contains(url, "tableau-dev.traffk.com");
            }
        }

        [TestClass]
        public class AddUserToSiteMethodTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenGivenTenantNameCreateSiteAndAddUser()
            {
                Random rnd = new Random();
                int tenantNumber = rnd.Next(1, 50);
                var siteName = "Test Tenant" + tenantNumber;
                var testService = new TableauRestService(new TrustedTicketGetter(Options), Options);
                string url;
                var site = ((ITableauRestService)testService).CreateSite(siteName, out url);
                Assert.IsNotNull(site);
                StringAssert.Contains(url, "tableau-dev.traffk.com");

                var newSiteOptions = MockEnvironment.TableauSignInOptions(url,
                    "Darren Alfonso", "DarrenTraffkTableau").Object;
                testService = new TableauRestService(new TrustedTicketGetter(newSiteOptions), newSiteOptions);
                ((ITableauRestService)testService).AddUserToSite(site.Id, "Test");
            }
        }

        [TestClass]
        public class DownloadWorkbookMethodTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenGivenListOfSingleWorkbookDownloadWorkbook()
            {
                var testService = new TableauRestService(new TrustedTicketGetter(Options), Options);
                var workbookRequest = ((ITableauRestService) testService).DownloadWorkbooksList();
                workbookRequest.ExecuteRequest();
                var workbooks = workbookRequest.Workbooks;
            }
        }

        //[TestClass]
        //public class SignInMethodTests : TableauRestServiceTests
        //{
        //    [TestMethod]
        //    public void WhenGivenUrlSignIn()
        //    {
        //        var testService = new TableauRestService(Options);
        //        Assert.IsNotNull(testService);
        //    }
        //}

        //[TestClass]
        //public class DownloadProjectsListMethodTests
        //{
        //    [TestMethod]
        //    public void WhenSignedInDownloadAllProjects()
        //    {
        //        var testService = new TableauRestService();
        //        var testUrls = TableauServerUrls.FromContentUrl("http://traffk-dev-tab.eastus.cloudapp.azure.com/#/projects", 1);
        //        var signIn = testService.SignIn(testUrls, "Test", "TraffkTestTableau");
        //        var projects = testService.DownloadProjectsList(testUrls, signIn);
        //        Assert.IsNotNull(projects);
        //        Assert.IsTrue(projects.Projects.Any());
        //    }
        //}

        //[TestClass]
        //public class DownloadViewsFromSiteMethodTests
        //{
        //    [TestMethod]
        //    public void WhenSignedInDownloadAllViews()
        //    {
        //        var testService = new TableauRestService();
        //        var testUrls = TableauServerUrls.FromContentUrl("http://traffk-dev-tab.eastus.cloudapp.azure.com/#/projects/1/", 50);
        //        var signIn = testService.SignIn(testUrls, "Test", "TraffkTestTableau");
        //        var views = testService.DownloadViewsForSite(testUrls, signIn);
        //        Assert.IsNotNull(views);
        //        Assert.IsTrue(views.Views.Any());
        //    }
        //}

        //[TestClass]
        //public class DownloadPreviewImageForViewMethodTests
        //{
        //    [TestMethod]
        //    public void WhenSignedInDownloadPreviewImage()
        //    {
        //        var testService = new TableauRestService();
        //        var testUrls = TableauServerUrls.FromContentUrl("http://traffk-dev-tab.eastus.cloudapp.azure.com/#/projects/1/", 50);
        //        var signIn = testService.SignIn(testUrls, "Test", "TraffkTestTableau");
        //        var testImageBytes = testService.DownloadPreviewImageForView(testUrls, "6d8f31d9-aceb-40be-867d-1c980215b246",
        //            "c8922aac-c202-446b-8ed2-ff4dde96eaba", signIn);

        //        Assert.IsNotNull(testImageBytes);
        //    }
        //}

        //[TestClass]
        //public class GetUnderlyingDataMethodTests : TableauRestServiceTests
        //{
        //    [Ignore]
        //    [TestMethod]
        //    public void WhenSignedInGetUnderlyingData()
        //    {
        //        var testUrl = "https://tableau-dev.traffk.com/#/";
        //        var testUrlRaw = "https://tableau-dev.traffk.com";
        //        var testUser = "Test";
        //        var testPassword = "TraffkTestTableau";
        //        var testService = new TableauRestService();
        //        var testUrls = TableauServerUrls.FromContentUrl(testUrl, 50);
        //        var signIn = testService.SignIn(testUrls, testUser, testPassword);
        //        testService.GetUnderlyingData(MockEnvironment.TableauSignInOptions(testUrlRaw, testUser, testPassword).Object.Value, "AverageRiskMap", "AverageRiskDashboard", signIn);
        //    }
        //}
    }
}
