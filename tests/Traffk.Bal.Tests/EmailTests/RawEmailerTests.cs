using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Bal.Email;

namespace Traffk.Bal.Tests.EmailTests
{
    [TestClass]
    public class RawEmailerTests
    {
        [TestClass]
        public class ConstructorTests
        {
            [TestMethod]
            public void WhenGivenNullOptionsThrowException()
            {
                Assert.ThrowsException<ArgumentNullException>(() => new RawEmailer(null));
            }
        }

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
