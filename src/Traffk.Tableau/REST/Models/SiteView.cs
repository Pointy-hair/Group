using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using RevolutionaryStuff.Core.ApplicationParts;

namespace Traffk.Tableau.REST
{
    public class SiteViewResource : IName
    {
        public string Id { get; set; }

        string IName.Name => Id;

        public override string ToString() => Id;

    }

    public class SiteView : SiteViewResource
    {
        public readonly string Name;
        public readonly string ContentUrl;
        public readonly string WorkbookName;
        public readonly string WorkbookId;
        public readonly string OwnerId;
        public readonly string ViewName;

        public SiteView(XmlNode content)
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
        }

        private readonly Regex ContentUrlPattern = new Regex(@"^(?<workbook>.*?)\/sheets\/(?<view>.*?)$");

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

    public class SiteViewViewModel : SiteViewResource
    {
        public string Name { get; set; }
        public string WorkbookName { get; set; }
        public string ViewName { get; set; }
    }

    public class SiteViewFolderResource : SiteViewResource, IName
    {
        public string Name => Id;

        public override string ToString() => Id;

        public SiteViewFolderResource(string name)
        {
            base.Id = name;
        }
    }
}
