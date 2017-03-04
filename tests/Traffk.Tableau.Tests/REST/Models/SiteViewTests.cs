using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Traffk.Tableau.REST;
using Traffk.Tableau.REST.RestRequests;
using Moq;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.Tests.REST.Models
{
    [TestClass]
    public class SiteViewTests
    {
        [TestClass]
        public class ConstructorMethods
        {
            [TestMethod]
            public void WhenGivenXmlNodeReturnWorkbookName()
            {
                XDocument testXDocument = XDocument.Load("../Traffk.Tableau.Tests/REST/Models/TestViewResponse.xml");

                var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline", "http://tableau.com/api");
                var ns = nsManager.LookupNamespace("iwsOnline");
                var viewElements = testXDocument.Root.Descendants(XName.Get("view", ns));
                var testSiteView = new TableauReportVisual(viewElements.First().ToXmlNode(), ns);

                Assert.AreEqual(testSiteView.WorkbookName, "Finance");
            }

            [TestMethod]
            public void WhenGivenXmlNodeReturnViewName()
            {
                XDocument testXDocument = XDocument.Load("../Traffk.Tableau.Tests/REST/Models/TestViewResponse.xml");

                var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline", "http://tableau.com/api");
                var ns = nsManager.LookupNamespace("iwsOnline");
                var viewElements = testXDocument.Root.Descendants(XName.Get("view", ns));
                var testSiteView = new TableauReportVisual(viewElements.First().ToXmlNode(), ns);

                Assert.AreEqual(testSiteView.ViewName, "EconomicIndicators");
            }
        }

    }
}
