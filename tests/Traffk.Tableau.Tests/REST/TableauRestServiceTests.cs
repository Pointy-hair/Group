using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;
using Moq;
using Traffk.Tableau.REST.Models;

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
            [Ignore]
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
            [Ignore]
            [TestMethod]
            public void WhenGivenListOfSingleWorkbookDownloadWorkbook()
            {
                var testService = new TableauRestService(new TrustedTicketGetter(Options), Options);
                var workbooks = ((ITableauRestService) testService).DownloadWorkbooksList();

                var workbooksToDownload = new List<SiteWorkbook> {workbooks.SingleOrDefault(x => x.Name == "MyFirstReport")};



                string path = System.IO.Path.GetTempPath(); //Path.GetPathRoot(Directory.GetCurrentDirectory());
                path = path + @"TableauTestDownloadWorkbookFiles";
                var downloadedWorkbooks = ((ITableauRestService) testService).DownloadWorkbooks(workbooksToDownload, path, false);
                Assert.IsNotNull(downloadedWorkbooks);
                Assert.AreNotEqual(downloadedWorkbooks.Count, 0);
            }
        }

        [TestClass]
        public class UploadWorkbookMethodTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenPathUploadWorkbooks()
            {
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestTenant36", "Darren Alfonso", "DarrenTraffkTableau").Object;
                var testService = new TableauRestService(new TrustedTicketGetter(newSiteOptions), newSiteOptions);

                string path = System.IO.Path.GetTempPath();
                path = path + @"TableauUploadTestWorkbooks";
                ((ITableauRestService)testService).UploadWorkbooks("Default", "Darren",
                    "1keylimecakeballs2MAGICBARS3currentjellycookies4", true, path);
            }
        }

        [TestClass]
        public class DownloadDatasourceMethodsTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenSiteDownloadDatasourceList()
            {
                var testOptions = MockEnvironment.TableauSignInOptions().Object;
                var testService = new TableauRestService(new TrustedTicketGetter(testOptions), testOptions);

                var sources = ((ITableauRestService) testService).DownloadDatasourceList();
                Assert.IsNotNull(sources);
                Assert.AreEqual(1, sources.Count);
            }

            [Ignore]
            [TestMethod]
            public void WhenGivenSiteDownloadDatasourceAndConnections()
            {
                var testOptions = MockEnvironment.TableauSignInOptions().Object;
                var testService = new TableauRestService(new TrustedTicketGetter(testOptions), testOptions);

                var sources = ((ITableauRestService)testService).DownloadDatasourceList();
                Assert.IsNotNull(sources);
                Assert.AreEqual(1, sources.Count);

                var connections =
                    ((ITableauRestService) testService).DownloadConnectionsForDatasource(sources.First().Id);
                Assert.IsNotNull(connections);
                Assert.AreEqual(1, connections.Count);
            }

            [Ignore]
            [TestMethod]
            public void WhenGivenSiteDownloadDatasourceFiles()
            {
                var testOptions = MockEnvironment.TableauSignInOptions().Object;
                var testService = new TableauRestService(new TrustedTicketGetter(testOptions), testOptions);
                var sources = ((ITableauRestService)testService).DownloadDatasourceList();
                string path = System.IO.Path.GetTempPath();
                path = path + @"TableauTestDownloadDatasourceFiles";
                ((ITableauRestService) testService).DownloadDatasourceFiles(sources, path);
            }
        }

        [TestClass]
        public class UploadDatasourceMethodsTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenDatasourceFileUpload()
            {
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestTenant36", "Darren Alfonso", "DarrenTraffkTableau").Object;
                var testService = new TableauRestService(new TrustedTicketGetter(newSiteOptions), newSiteOptions);

                string path = System.IO.Path.GetTempPath();
                path = path + @"TableauUploadTestFiles";
                ((ITableauRestService) testService).UploadDatasourceFiles("Default", "Darren",
                    "1keylimecakeballs2MAGICBARS3currentjellycookies4", true, path);
            }

            [Ignore]
            [TestMethod]
            public void WhenGivenDatasourceAndServerAddressUpdate()
            {
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestTenant36", "Darren Alfonso", "DarrenTraffkTableau").Object;
                var testService = new TableauRestService(new TrustedTicketGetter(newSiteOptions), newSiteOptions) as ITableauRestService;

                var datasources = testService.DownloadDatasourceList();
                var datasourceToUpdate = datasources.First();

                var connection = testService.DownloadConnectionsForDatasource(datasourceToUpdate.Id).First();

                ((ITableauRestService)testService).UpdateDatasourceConnection(datasourceToUpdate, connection, connection.ServerAddress + @"/test");
            }
        }

        [TestClass]
        public class CreateNewTenantMethodsTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenCreateTenantRequestCreate()
            {
                var testMasterOptions = MockEnvironment.TableauSignInOptions().Object;
                var testMasterService =
                    new TableauRestService(new TrustedTicketGetter(testMasterOptions), testMasterOptions) as
                        ITableauRestService;

                var dbUserName = "Darren";
                var dbPassword = "1keylimecakeballs2MAGICBARS3currentjellycookies4";
                var testNewServerAddress = "traffkrdb-dev.database.windows.net/test";
                string path = System.IO.Path.GetTempPath();
                path = path + @"TableauIntegrationTestFiles";

                var testCreateTenantRequest = new CreateTableauTenantRequest("IntegrationTestTenant", dbUserName, dbPassword, testNewServerAddress, dbUserName, dbPassword, path);

                testMasterService.CreateNewTableauTenant(testCreateTenantRequest);
                
                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
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
