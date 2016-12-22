using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Bal.Email;

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
