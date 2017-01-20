using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly string WorkbookId;
        public readonly string OwnerId;

        public SiteView(XmlNode content)
        {
            if (content.Name.ToLower() != "view")
            {
                throw new Exception("Unexpected content - not view");
            }

            this.Id = content.Attributes["id"].Value;
            this.Name = content.Attributes["name"].Value;
            this.ContentUrl = content.Attributes["contentUrl"].Value;
        }
    }

    public class SiteViewFolderResource : SiteViewResource, IName
    {
        string IName.Name => Id;

        public override string ToString() => Id;

        public SiteViewFolderResource(string name)
        {
            base.Id = name;
        }
    }
}
