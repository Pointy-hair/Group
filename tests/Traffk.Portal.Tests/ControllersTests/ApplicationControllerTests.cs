using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Portal;
using TraffkPortal;
using TraffkPortal.Controllers;
using Moq;
using Traffk.Bal.Services;
using Traffk.Bal.Data.Rdb;
using TraffkPortal.Services;
using Microsoft.Extensions.Logging;
using TraffkPortal.Models.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Traffk.Portal.Tests.Helpers;
using static Traffk.Bal.Services.BlobStorageServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Traffk.Bal;

namespace Traffk.Portal.Tests.ControllersTests
{
    [TestClass]
    public class ApplicationControllerTests
    {
        [TestClass]
        public class ApplicationControllerMocks
        {
            public Mock<BlobStorageServices> MockBlobStorage { get; set; }
            public Mock<TraffkRdbContext> MockRdbContext { get; set; }
            public Mock<CurrentContextServices> MockCurrentContextServices { get; set; }
            public Mock<ILoggerFactory> MockLoggerFactory { get; set; }

            public ApplicationControllerMocks()
            {
                //var mockITraffkTenantFinder = new Mock<ITraffkTenantFinder>();

                //var mockIHostingEnvironment = new Mock<IHostingEnvironment>();
                //var mockConfigStringFormatter = new Mock<ConfigStringFormatter>(mockIHostingEnvironment.Object, mockITraffkTenantFinder.Object);

                //var mockDbContextOptions = new DbContextOptions<TraffkRdbContext>();
                //MockRdbContext = new Mock<TraffkRdbContext>(mockDbContextOptions, mockITraffkTenantFinder.Object, mockConfigStringFormatter.Object);

                //var mockCurrentTenantServices = new Mock<CurrentTenantServices>(mockITraffkTenantFinder.Object, MockRdbContext.Object);
                //var mockCurrentUser = new Mock<ICurrentUser>();
                //var mockBlobStorageServiceOptions = new Mock<IOptions<BlobStorageServicesOptions>>();
                //mockBlobStorageServiceOptions.Setup(x => x.Value).Returns(
                //    new BlobStorageServicesOptions
                //    {
                //        ConnectionString = "MockBlobStorageConnectionString"
                //    }
                //);
                
                //MockBlobStorage = new Mock<BlobStorageServices>(mockCurrentTenantServices.Object, mockCurrentUser.Object, mockBlobStorageServiceOptions.Object);
                //MockCurrentContextServices = new Mock<CurrentContextServices>();
                //MockLoggerFactory = new Mock<ILoggerFactory>();
            }
        }

        [TestClass]
        public class PortalSettingsMethodTests
        {
            [TestMethod]
            public void WhenGivenNonImageFileDoNotUploadFavicon()
            {
                ////Default mocks
                //var mocks = new ApplicationControllerMocks();
                //var testController = new ApplicationController(mocks.MockBlobStorage.Object,
                //    mocks.MockRdbContext.Object,
                //    mocks.MockCurrentContextServices.Object,
                //    mocks.MockLoggerFactory.Object);

                //var testApplication = new Application();
                //var mockApplicationDbset = DbSetMock.Create(testApplication);
                //mocks.MockRdbContext.Setup(x => x.Applications).Returns(mockApplicationDbset.Object);
                //mocks.MockRdbContext.Setup(x => x.Update(It.IsAny<object>()));
                //mocks.MockRdbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

                //mocks.MockBlobStorage.Setup(x => x.StoreFileAsync(It.IsAny<bool>(),
                //    It.IsAny<Roots>(),
                //    It.IsAny<IFormFile>(),
                //    It.IsAny<string>(),
                //    It.IsAny<bool>()))
                //    .Returns(new Task<Uri>(() => new Uri("http://traffk.com")));

                //var testViewModel = new PortalOptionsModel();
                //string folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/TestFiles/";
                //string testFileName = "TestExcel.xlsx";
                //string testFilePath = folder + testFileName;

                //FileStream fileStream = new FileStream(testFilePath, FileMode.Open);

                //Mock<IFormFile> uploadedFile = new Mock<IFormFile>();
                //uploadedFile.Setup(f => f.Length).Returns(Convert.ToInt16(fileStream.Length));
                //uploadedFile.Setup(f => f.FileName).Returns(testFileName);
                //uploadedFile.Setup(f => f.OpenReadStream()).Returns(fileStream);

                //testViewModel.FaviconFile = uploadedFile.Object;

                //var testResult = testController.PortalSettings(1, testViewModel);

            }
        }
    }
}
