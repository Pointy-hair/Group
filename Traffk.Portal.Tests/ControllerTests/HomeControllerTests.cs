﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Portal;

namespace Traffk.Portal.Tests.ControllerTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestClass]
        public class IndexMethodTests
        {
            [TestMethod]
            public void WhenGivenIndexReturnNotNull()
            {
                object nullObject = null;
                Assert.IsNotNull(nullObject);
            }
        }
    }
}