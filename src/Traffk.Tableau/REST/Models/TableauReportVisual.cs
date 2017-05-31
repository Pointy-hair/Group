using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using RevolutionaryStuff.Core.ApplicationParts;

namespace Traffk.Tableau.REST.Models
{
    public class TableauResource : IName
    {
        public string Id { get; set; }
        public string Name { get; set; }
        string IName.Name => Name;
        public override string ToString() => Id + "-" + Name;

    }

    public class TableauReportVisual : TableauResource, ITableauReportVisual
    {
        string ITableauReportVisual.Id => Id;
        string ITableauReportVisual.Title => Title;
        string ITableauReportVisual.ViewName => ViewName;
        string ITableauReportVisual.WorkbookId => WorkbookId;
        string ITableauReportVisual.WorkbookName => WorkbookName;
        string ITableauReportVisual.WorksheetName => WorksheetName;
        public string Title => Name;
        public string WorkbookName { get; set; }
        public string WorksheetName { get; set; }
        public string ViewName { get; set; }
        public readonly string ContentUrl;
        public string WorkbookId { get; }
        public readonly string OwnerId;
        public string Description = "";//"No desc, Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

        public TableauReportVisual()
        {
            
        }

        public TableauReportVisual(XmlNode content, string xmlNamespace)
        {
            if (content.Name.ToLower() != "view")
            {
                throw new Exception("Unexpected content - not view");
            }

            this.Id = content.Attributes["id"].Value;
            this.Name = content.Attributes["name"].Value;
            this.ContentUrl = content.Attributes["contentUrl"].Value;
            this.WorkbookName = GetWorkbookName(ContentUrl);
            this.ViewName = GetViewName(ContentUrl);
            this.WorkbookId = GetWorkbookId(content, xmlNamespace);
        }

        private readonly Regex ContentUrlPattern = new Regex(@"^(?<workbook>.*?)\/sheets\/(?<view>.*?)$");

        private string GetWorkbookId(XmlNode content, string xmlNamespace)
        {
            var contentElement = content.ToXElement();
            var workbookNode = contentElement.Descendants(XName.Get("workbook", xmlNamespace)).First().ToXmlNode();
            var workbookId = workbookNode.Attributes["id"].Value;
            return workbookId;
        }

        private string GetWorkbookName(string contentUrl)
        {
            Match match = ContentUrlPattern.Match(contentUrl);
            string workbook = match.Groups["workbook"].Value;
            return workbook;
        }

        private string GetViewName(string contentUrl)
        {
            Match match = ContentUrlPattern.Match(contentUrl);
            string view = match.Groups["view"].Value;
            return view;
        }

    }

    public class TableauContactReportVisual : TableauReportVisual, ITableauContactReportVisual
    {
        public string ContactId { get; set; }
    }

    public class TableauFolder : TableauResource, IName
    {
        public TableauFolder(string name, string id = "")
        {
            base.Id = id;
            base.Name = name;
        }
    }
}
