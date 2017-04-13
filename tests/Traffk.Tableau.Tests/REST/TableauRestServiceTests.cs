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
using Traffk.Tableau.REST.Helpers;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.Tests.REST
{
    [TestClass]
    public class TableauRestServiceTests
    {
        public IOptions<TableauSignInOptions> Options { get; set; }
        public ITableauTenantFinder TableauTenantFinder { get; set; }
        public ITableauUserCredentials TableauAdminCredentials { get; set; }

        public TableauRestServiceTests()
        {
            Options = MockEnvironment.TableauSignInOptions().Object;
            TableauTenantFinder = MockEnvironment.TableauTenantFinder().Object;
            TableauAdminCredentials = MockEnvironment.TableauAdminCredentials().Object;
        }

        [TestClass]
        public class CreateSiteMethodTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenTenantNameCreateSite()
            {
                var testService = new TableauRestService(Options, TableauAdminCredentials);
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
                //Random rnd = new Random();
                //int tenantNumber = rnd.Next(1, 50);
                //var siteName = "Test Tenant" + tenantNumber;
                //var testService = new TableauRestService(Options, TableauAdminCredentials);
                //string url;
                //var site = ((ITableauRestService)testService).CreateSite(siteName, out url);
                //Assert.IsNotNull(site);
                //StringAssert.Contains(url, "tableau-dev.traffk.com");

                //var newSiteOptions = MockEnvironment.TableauSignInOptions(url,
                //    "Darren Alfonso", "DarrenTraffkTableau").Object;
                
                //testService = new TableauRestService(n);
                //((ITableauRestService)testService).AddUserToSite(site.Id, "Test");
            }
        }

        [TestClass]
        public class DownloadWorkbookMethodTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenListOfSingleWorkbookDownloadWorkbook()
            {
                var testService = new TableauRestService(Options,TableauAdminCredentials);
                var workbooks = ((ITableauRestService)testService).DownloadWorkbooksList();

                var workbooksToDownload = new List<SiteWorkbook> { workbooks.SingleOrDefault(x => x.Name == "MyFirstReport") };



                string path = System.IO.Path.GetTempPath(); //Path.GetPathRoot(Directory.GetCurrentDirectory());
                path = path + @"TableauTestDownloadWorkbookFiles";
                var downloadedWorkbooks = ((ITableauRestService)testService).DownloadWorkbooks(workbooksToDownload, path, false);
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
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestTenant36").Object;
                var testService = new TableauRestService(newSiteOptions, TableauAdminCredentials);

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
                var testService = new TableauRestService(Options, TableauAdminCredentials);

                var sources = ((ITableauRestService)testService).DownloadDatasourceList();
                Assert.IsNotNull(sources);
                Assert.AreEqual(1, sources.Count);
            }

            [Ignore]
            [TestMethod]
            public void WhenGivenSiteDownloadDatasourceAndConnections()
            {
                var testService = new TableauRestService(Options, TableauAdminCredentials);

                var sources = ((ITableauRestService)testService).DownloadDatasourceList();
                Assert.IsNotNull(sources);
                Assert.AreEqual(1, sources.Count);

                var connections =
                    ((ITableauRestService)testService).DownloadConnectionsForDatasource(sources.First().Id);
                Assert.IsNotNull(connections);
                Assert.AreEqual(1, connections.Count);
            }

            [Ignore]
            [TestMethod]
            public void WhenGivenSiteDownloadDatasourceFiles()
            {
                var testService = new TableauRestService(Options, TableauAdminCredentials);

                var sources = ((ITableauRestService)testService).DownloadDatasourceList();
                string path = System.IO.Path.GetTempPath();
                path = path + @"TableauTestDownloadDatasourceFiles";
                ((ITableauRestService)testService).DownloadDatasourceFiles(sources, path);
            }
        }

        [TestClass]
        public class UploadDatasourceMethodsTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenGivenDatasourceFileUpload()
            {
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestTenant36").Object;
                var testService = new TableauRestService(newSiteOptions, TableauAdminCredentials);

                string path = System.IO.Path.GetTempPath();
                path = path + @"TableauUploadTestFiles";
                ((ITableauRestService)testService).UploadDatasourceFiles("Default", "Darren",
                    "1keylimecakeballs2MAGICBARS3currentjellycookies4", true, path);
            }

            [Ignore]
            [TestMethod]
            public void WhenGivenDatasourceAndServerAddressUpdate()
            {
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestTenant36").Object;
                var testService = new TableauRestService(newSiteOptions, TableauAdminCredentials) as ITableauRestService;

                var datasources = testService.DownloadDatasourceList();
                var datasourceToUpdate = datasources.First();

                var connection = testService.DownloadConnectionsForDatasource(datasourceToUpdate.Id).First();

                ((ITableauRestService)testService).UpdateDatasourceConnection(datasourceToUpdate, connection, connection.ServerAddress + @"/test");
            }

            [TestMethod]
            public void WhenGivenDatasourceUpdateDbName()
            {
                string path = System.IO.Path.GetTempPath();
                path = path + @"TableauUploadTestFiles\MyAddressTable.tdsx";

                TableauFileEditor.UpdateDatasourceDatabaseName(path, "TraffkHip2", path);
            }

            [TestMethod]
            public void WhenGivenReportFileUpdateSiteReferences()
            {
                //string path = System.IO.Path.GetTempPath();
                //path = path + @"TableauUploadTestFiles\MyFirstReport.twb";

                //TableauFileEditor.UpdateWorkbookFileSiteReferences(path);
            }
        }

        [TestClass]
        public class CreateNewTenantMethodsTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenGivenCreateTenantRequestCreate()
            {
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestMasterTenant").Object;
                var testMasterService =
                    new TableauRestService(newSiteOptions, TableauAdminCredentials) as
                        ITableauRestService;

                var dbUserName = "Darren";
                var dbPassword = "1keylimecakeballs2MAGICBARS3currentjellycookies4";
                var testNewServerAddress = "traffkrdb-dev.database.windows.net/";
                var testDbName = "TraffkHip2";
                string path = System.IO.Path.GetTempPath();
                path = path + @"TableauIntegrationTestFiles";

                var testCreateTenantRequest = new CreateTableauTenantRequest("IntegrationTestTenant", dbUserName, dbPassword, testNewServerAddress, testDbName, dbUserName, dbPassword, path);

                testMasterService.CreateNewTableauTenant(testCreateTenantRequest);

                DirectoryInfo di = new DirectoryInfo(path);

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


        [TestClass]
        public class SignInMethodTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenGivenUrlSignIn()
            {
                var testService = new TableauRestService(Options, TableauAdminCredentials);
                Assert.IsNotNull(testService);
            }
        }

        [TestClass]
        public class DownloadProjectsListMethodTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenSignedInDownloadAllProjects()
            {
                var testService = new TableauRestService(Options, TableauAdminCredentials);
                var projects = testService.DownloadProjectsList();
                Assert.IsNotNull(projects);
                Assert.IsTrue(projects.Projects.Any());
            }
        }

        [TestClass]
        public class DownloadViewsFromSiteMethodTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenSignedInDownloadAllViews()
            {
                var testService = new TableauRestService(Options, TableauAdminCredentials) as ITableauRestService;
                var views = testService.DownloadViewsForSite();
                Assert.IsNotNull(views);
                Assert.IsTrue(views.Views.Any());
            }
        }

        [TestClass]
        public class DownloadPreviewImageForViewMethodTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenSignedInDownloadPreviewImage()
            {
                var testService = new TableauRestService(Options, TableauAdminCredentials) as ITableauRestService;
                var testImageBytes = testService.DownloadPreviewImageForView("6d8f31d9-aceb-40be-867d-1c980215b246","c8922aac-c202-446b-8ed2-ff4dde96eaba");

                Assert.IsNotNull(testImageBytes);
            }
        }

        [TestClass]
        public class GetUnderlyingDataMethodTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenSignedInGetUnderlyingData()
            {
                
            }
        }
    }
}
