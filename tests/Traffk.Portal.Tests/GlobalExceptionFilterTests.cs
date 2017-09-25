using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TraffkPortal.Services;

namespace Traffk.Portal.Tests
{
    [TestClass]
    public class GlobalExceptionFilterTests
    {
        [TestClass]
        public class OnExceptionMethodTests
        {
            [TestMethod]
            public void WhenGivenExceptionDoNotThrowAnotherException()
            {
                var testFilter = new GlobalExceptionFilter();
                Assert.ThrowsException<ArgumentNullException>(() => testFilter.OnException(new ExceptionContext(new ActionContext(), new List<IFilterMetadata>())));
            }
            
        }
    }
}
