using System;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.Tests.REST
{
    [TestClass]
    public class TableauRestServiceTests
    {
        public IOptions<TableauSignInOptions> Options { get; set; }
        public ITableauTenantFinder TableauTenantFinder { get; set; }
        public TableauAdminCredentials TableauAdminCredentials { get; set; }
        public ITableauUserCredentials TableauUserCredentials { get; set; }

    public TableauRestServiceTests()
        {
            Options = MockEnvironment.TableauSignInOptions().Object;
            TableauTenantFinder = MockEnvironment.TableauTenantFinder().Object;
            var tableauAdminCredentials = MockEnvironment.TableauAdminCredentials().Object;
            TableauAdminCredentials = new TableauAdminCredentials
            {
                Username = tableauAdminCredentials.UserName,
                Password = tableauAdminCredentials.Password
            };
            TableauUserCredentials = MockEnvironment.TableauUserCredentials().Object;
        }

        [TestClass]
        public class CreateSiteMethodTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenTenantNameCreateSite()
            {
                //Needed to comment this out because after testing, we made method-under-test protected

                //var testService = new TableauAdminService(Options, TableauAdminCredentials);
                //var site = TableauAdminService.CreateSite("Test Tenant 419");
                //Assert.IsNotNull(site);
            }
        }

        [TestClass]
        public class AddUserToSiteMethodTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenTenantNameCreateSiteAndAddUser()
            {
                //Needed to comment this out because after testing, we made method-under-test protected

                //var siteName = "CreateSiteAndAddUserTestTenant";
                //var testService = new TableauAdminService(Options, TableauAdminCredentials);
                //var site = testService.CreateSite(siteName);

                //Assert.IsNotNull(site);

                //var newSiteOptions = MockEnvironment.TableauSignInOptions().Object;
                //newSiteOptions.Value.UpdateForTenant(site.TenantId);

                //var newSiteTestService = new TableauAdminService(newSiteOptions, TableauAdminCredentials) as ITableauAdminService;
                //newSiteTestService.AddUserToSite(site.Id, "Test");
            }
        }

        //[TestClass]
        //public class DownloadWorkbookMethodTests : TableauRestServiceTests
        //{
        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenListOfSingleWorkbookDownloadWorkbookFile()
        //    {
        //        var testService = new TableauUserService(Options,TableauAdminCredentials);
        //        var workbooks = ((ITableauUserService)testService).DownloadWorkbooksList();

        //        var workbooksToDownload = new List<SiteWorkbook> { workbooks.SingleOrDefault(x => x.Name == "MyFirstReport") };



        //        string path = System.IO.Path.GetTempPath(); //Path.GetPathRoot(Directory.GetCurrentDirectory());
        //        path = path + @"TableauTestDownloadWorkbookFiles";
        //        var downloadedWorkbooks = ((ITableauUserService)testService).DownloadWorkbookFiles(workbooksToDownload, path, false);
        //        Assert.IsNotNull(downloadedWorkbooks);
        //        Assert.AreNotEqual(downloadedWorkbooks.Count, 0);
        //    }
        //}

        //[TestClass]
        //public class UploadWorkbookMethodTests : TableauRestServiceTests
        //{
        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenPathUploadWorkbooks()
        //    {
        //        var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestMasterTenant").Object;
        //        var testService = new TableauUserService(newSiteOptions, TableauAdminCredentials);

        //        string path = System.IO.Path.GetTempPath();
        //        path = path + @"TableauUploadTestWorkbooks";
        //        ((ITableauUserService)testService).UploadWorkbooks("Default", "Darren",
        //            "1keylimecakeballs2MAGICBARS3currentjellycookies4", true, path);
        //    }
        //}

        //[TestClass]
        //public class DownloadDatasourceMethodsTests : TableauRestServiceTests
        //{
        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenSiteDownloadDatasourceList()
        //    {
        //        var testService = new TableauUserService(Options, TableauAdminCredentials);

        //        var sources = ((ITableauUserService)testService).DownloadDatasourceList();
        //        Assert.IsNotNull(sources);
        //        Assert.AreEqual(1, sources.Count);
        //    }

        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenSiteDownloadDatasourceAndConnections()
        //    {
        //        var testService = new TableauUserService(Options, TableauAdminCredentials);

        //        var sources = ((ITableauUserService)testService).DownloadDatasourceList();
        //        Assert.IsNotNull(sources);
        //        Assert.AreEqual(1, sources.Count);

        //        var connections =
        //            ((ITableauUserService)testService).DownloadConnectionsForDatasource(sources.First().Id);
        //        Assert.IsNotNull(connections);
        //        Assert.AreEqual(1, connections.Count);
        //    }

        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenSiteDownloadDatasourceFiles()
        //    {
        //        var testService = new TableauUserService(Options, TableauAdminCredentials);

        //        var sources = ((ITableauUserService)testService).DownloadDatasourceList();
        //        string path = System.IO.Path.GetTempPath();
        //        path = path + @"TableauTestDownloadDatasourceFiles";
        //        ((ITableauUserService)testService).DownloadDatasourceFiles(sources, path);
        //    }
        //}

        //[TestClass]
        //public class UploadDatasourceMethodsTests : TableauRestServiceTests
        //{
        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenDatasourceFileUpload()
        //    {
        //        var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestMasterTenant").Object;
        //        var testService = new TableauUserService(newSiteOptions, TableauAdminCredentials);

        //        string path = System.IO.Path.GetTempPath();
        //        path = path + @"TableauUploadTestFiles";
        //        ((ITableauUserService)testService).UploadDatasourceFiles("Default", "Darren",
        //            "1keylimecakeballs2MAGICBARS3currentjellycookies4", true, path);
        //    }

        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenDatasourceAndServerAddressUpdate()
        //    {
        //        var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestMasterTenant").Object;
        //        var testService = new TableauUserService(newSiteOptions, TableauAdminCredentials) as ITableauUserService;

        //        var datasources = testService.DownloadDatasourceList();
        //        var datasourceToUpdate = datasources.First();

        //        var connection = testService.DownloadConnectionsForDatasource(datasourceToUpdate.Id).First();

        //        ((ITableauUserService)testService).UpdateDatasourceConnection(datasourceToUpdate, connection, connection.ServerAddress + @"/test");
        //    }

        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenDatasourceUpdateDbName()
        //    {
        //        string path = System.IO.Path.GetTempPath();
        //        path = path + @"TableauUploadTestFiles\MyAddressTable.tdsx";

        //        TableauFileEditor.UpdateDatasourceDatabaseName(path, "TraffkHip2", path);
        //    }

        //    [Ignore]
        //    [TestMethod]
        //    public void WhenGivenReportFileUpdateSiteReferences()
        //    {
        //        //string path = System.IO.Path.GetTempPath();
        //        //path = path + @"TableauUploadTestFiles\MyFirstReport.twb";

        //        //TableauFileEditor.UpdateWorkbookFileSiteReferences(path);
        //    }
        //}

        [TestClass]
        public class CreateNewTenantMethodsTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenCreateTenantRequestCreate()
            {
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestMasterTenant").Object;
                var tableauAdminCredentials = ConfigurationHelpers.CreateOptions(TableauAdminCredentials);
                var testAdminService =
                    new TableauAdminService(newSiteOptions, tableauAdminCredentials) as ITableauAdminService;

                var dbUserName = "Darren";
                var dbPassword = "1keylimecakeballs2MAGICBARS3currentjellycookies4";
                var testNewServerAddress = "traffkrdb-dev.database.windows.net/";
                var testDbName = "TraffkHip2";
                string path = Path.GetTempPath();
                path = path + @"TableauIntegrationTestFiles";

                var testCreateTenantRequest = new CreateTableauTenantRequest("CreateAndMigrateTestTenant", dbUserName, dbPassword, testNewServerAddress, testDbName, dbUserName, dbPassword, path);

                testAdminService.CreateTableauTenant(testCreateTenantRequest);

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
        public class MigrateDataMethodsTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenGivenExistingSiteMigrateData()
            {
                var newSiteOptions = MockEnvironment.TableauSignInOptions("https://tableau-dev.traffk.com/#/site/TestMasterTenant").Object;
                var tableauAdminCredentials = ConfigurationHelpers.CreateOptions(TableauAdminCredentials);

                var testAdminService =
                    new TableauAdminService(newSiteOptions, tableauAdminCredentials) as ITableauAdminService;

                var dbUserName = "Darren";
                var dbPassword = "1keylimecakeballs2MAGICBARS3currentjellycookies4";
                var testNewServerAddress = "traffkrdb-dev.database.windows.net/";
                var testDbName = "TraffkHip2";
                string path = Path.GetTempPath();
                path = path + @"TableauIntegrationTestFiles";

                var dataMigrationRequest = new TableauDataMigrationRequest("ExistingTestTenant", testNewServerAddress, testDbName, dbUserName, dbPassword, path);

                testAdminService.MigrateDataset(dataMigrationRequest);

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
                var testService = new TableauViewerService(Options, TableauUserCredentials);
                Assert.IsNotNull(testService);

                var tableauAdminCredentials = ConfigurationHelpers.CreateOptions(TableauAdminCredentials);

                var testAdminService = new TableauAdminService(Options, tableauAdminCredentials);
                Assert.IsNotNull(testAdminService);
            }
        }

        //[TestClass]
        //public class DownloadProjectsListMethodTests : TableauRestServiceTests
        //{
        //    [TestMethod]
        //    public void WhenSignedInDownloadProjectList()
        //    {
        //        var testService = new TableauUserService(Options, TableauAdminCredentials);
        //        var projects = testService.DownloadProjectsList();
        //        Assert.IsNotNull(projects);
        //        Assert.IsTrue(projects.Projects.Any());
        //    }
        //}

        [TestClass]
        public class DownloadViewsListMethodTests : TableauRestServiceTests
        {
            [TestMethod]
            public void WhenSignedInDownloadViewList()
            {
                var testService = new TableauViewerService(Options, TableauUserCredentials) as ITableauViewerService;
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
                var testService = new TableauViewerService(Options, TableauUserCredentials) as ITableauViewerService;
                var testImageBytes = testService.DownloadPreviewImageForView("6d8f31d9-aceb-40be-867d-1c980215b246", "c8922aac-c202-446b-8ed2-ff4dde96eaba");

                Assert.IsNotNull(testImageBytes);
            }
        }

        [TestClass]
        public class GetUnderlyingDataMethodTests : TableauRestServiceTests
        {
            [Ignore]
            [TestMethod]
            public void WhenSignedInGetUnderlyingDataUsingWorksheetName()
            {
                var trustedTicketGetter = new TrustedTicketGetter(Options, TableauAdminCredentials, TableauTenantFinder);
                var testService = new TableauVisualServices(trustedTicketGetter, Options) as ITableauVisualServices;

                var worksheetName = "Risk by Occupation";
                var testGetUnderlyingDataOptions = new GetUnderlyingDataOptions {WorksheetName = worksheetName };
                var data = testService.GetUnderlyingDataAsync(testGetUnderlyingDataOptions, "RiskIndex-Occupations",
                    "EmploymentandRisk").ExecuteSynchronously();
                var dataTable = data.ToDataTable();

                Assert.IsNotNull(data);
                Assert.IsNotNull(dataTable);
            }

            [Ignore]
            [TestMethod]
            public void WhenSignedInGetUnderlyingDataUsingDashboardName()
            {
                var trustedTicketGetter = new TrustedTicketGetter(Options, TableauAdminCredentials, TableauTenantFinder);
                var testService = new TableauVisualServices(trustedTicketGetter, Options) as ITableauVisualServices;

                var dashboardName = "Employment and Risk";
                var testGetUnderlyingDataOptions = new GetUnderlyingDataOptions { DashboardName = dashboardName };
                var data = testService.GetUnderlyingDataAsync(testGetUnderlyingDataOptions, "RiskIndex-Occupations",
                    "EmploymentandRisk").ExecuteSynchronously();
                var dataTable = data.ToDataTable();

                Assert.IsNotNull(data);
                Assert.IsNotNull(dataTable);
            }

			[Ignore]
            [TestMethod]
            public async Task WhenSignedInGetUnderlyingDataWithManyRows()
            {
                var trustedTicketGetter = new TrustedTicketGetter(Options, TableauAdminCredentials, TableauTenantFinder);
                var testService = new TableauVisualServices(trustedTicketGetter, Options) as ITableauVisualServices;

                var worksheetName = "Avg Risk by Mbr Relationship";

                var testGetUnderlyingDataOptions = new GetUnderlyingDataOptions { WorksheetName = worksheetName };
                int timeout = 900000; //900000 ms = 15 minutes
                var task = testService.GetUnderlyingDataAsync(testGetUnderlyingDataOptions, "AverageRiskScore",
                    "1-AverageRiskScore_demo");

                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    var data = task.Result;
                    var dataTable = data.ToDataTable();
                    Assert.IsNotNull(data);
                    Assert.IsNotNull(dataTable);
                }
                else
                {
                    throw new Exception("Timed Out");
                }
            }
        }
    }
}
