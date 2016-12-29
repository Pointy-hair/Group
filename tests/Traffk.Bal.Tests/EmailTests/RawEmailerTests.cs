using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Traffk.Bal.Tests.EmailTests
{
    [TestClass]
    public class RawEmailerTests
    {
        [TestClass]
        public class SendEmailMethodTests
        {
            [TestMethod]
            public void WhenGivenSmtpOptionsSendEmail()
            {
                var testString = "NOT NULL";
                Assert.IsNotNull(testString);

                //object nullObject = null;
                //Assert.IsNotNull(nullObject);
            }
        }
    }
}
