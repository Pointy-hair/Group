using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Traffk.Bal.Services;
using Traffk.Bal.Data.Rdb;
using TraffkPortal.Services;
using Microsoft.Extensions.Logging;

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
                //CurrentTenantServices currentTenant, ICurrentUser currentUser, IOptions<BlobStorageServicesOptions> options
                //var mockCurrentTenantServices = new Mock<CurrentTenantServices>();
                //var mockCurrentUser = new Mock<ICurrentUser>();
                //var mockBlobStorageServiceOptions = new Mock<IOptions<BlobStorageServicesOptions>>();

                //MockBlobStorage = new Mock<BlobStorageServices>(mockCurrentTenantServices.Object, mockCurrentUser.Object, mockBlobStorageServiceOptions.Object);
                //MockRdbContext = new Mock<TraffkRdbContext>();
                //MockCurrentContextServices = new Mock<CurrentContextServices>();
                //MockLoggerFactory = new Mock<ILoggerFactory>();
            }
        }

        [TestClass]
        public class PortalSettingsMethodTests
        {
            [Ignore]
            [TestMethod]
            public void WhenGivenNonImageFileDoNotUploadFavicon()
            {
                //Default mocks
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
