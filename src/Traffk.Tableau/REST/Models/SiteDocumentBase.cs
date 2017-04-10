using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Traffk.Tableau.REST.Models
{
    /// <summary>
    /// Base class for information common to Workbooks and Data Sources, so we don't have lots of redundant code
    /// </summary>
    public class SiteDocumentBase : IHasProjectId, IHasSiteItemId
    {
        public readonly string Id;
        public readonly string Name;
        //Note: [2015-10-28] Datasources presently don't return this information
        //public readonly string ContentUrl;
        public readonly string ProjectId;
        public readonly string ProjectName;
        public readonly string OwnerId;
        public readonly SiteTagsSet TagsSet;

        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        protected SiteDocumentBase(XmlNode xmlNode, string xmlNamespace)
        {
            this.Name = xmlNode.Attributes["name"].Value;
            this.Id = xmlNode.Attributes["id"].Value;

//Note: [2015-10-28] Datasources presently don't return this information
//        this.ContentUrl = xmlNode.Attributes["contentUrl"].Value;

            //Namespace for XPath queries
            //var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");

            var xElement = xmlNode.ToXElement();

            //Get the project attributes
            var projectElement = xElement.Descendants(XName.Get("project", xmlNamespace)).First();
            var projectNode = projectElement.ToXmlNode();
            this.ProjectId = projectNode.Attributes["id"].Value;

            //Some responses do not have the project name
            var attrProjectName = projectNode.Attributes["name"];
            if(attrProjectName != null)
            {
                this.ProjectName = attrProjectName.Value;
            }  

            //Get the owner attributes
            var ownerElement = xElement.Descendants(XName.Get("owner", xmlNamespace)).First();
            var ownerNode = ownerElement.ToXmlNode();
            this.OwnerId = ownerNode.Attributes["id"].Value;

            //See if there are tags
            var tagsElement = xElement.Descendants(XName.Get("tags", xmlNamespace)).First();
            var tagsNode = tagsElement.ToXmlNode();
            if (tagsNode != null)
            {
                this.TagsSet = new SiteTagsSet(tagsNode, xmlNamespace);
            }
        }

        /// <summary>
        /// Space delimited list of tags
        /// </summary>
        public string TagSetText
        {
            get
            {
                var tagSet = this.TagsSet;
                if (tagSet == null) return "";
                return tagSet.TagSetText;
            }
        }

        public bool IsTaggedWith(string tagText)
        {
            var tagSet = this.TagsSet;
            if(tagSet == null)
            {
                return false;
            }
            return TagsSet.IsTaggedWith(tagText);
        }

        string IHasSiteItemId.Id
        {
            get { return this.Id; }
        }

        string IHasProjectId.ProjectId {
            get { return this.ProjectId; }
        }
    }
}
