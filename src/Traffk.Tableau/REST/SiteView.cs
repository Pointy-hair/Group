using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Traffk.Tableau.REST
{
    public class SiteView
    {
        public readonly string Id;
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

        }
    }
}
