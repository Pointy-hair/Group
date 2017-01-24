using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Traffk.Tableau.REST.Models
{
    /// <summary>
    /// Information about a Workbook in a Server's site
    /// </summary>
    public class SiteWorkbook : SiteDocumentBase
    {
        public readonly bool ShowTabs;
        //Note: [2015-10-28] Datasources presently don't return this information, so we need to make this workbook specific
        public readonly string ContentUrl;


        /// <summary>
        /// If set, contains the set of data connections embedded in this workbooks
        /// </summary>
        private List<SiteConnection> _dataConnections;

        public ReadOnlyCollection<SiteConnection> DataConnections
        {
            get
            {
                var dataConnections = _dataConnections;
                if (dataConnections == null) return null;

                return dataConnections.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workbookNode"></param>
        public SiteWorkbook(XmlNode workbookNode, string xmlNamespace) : base(workbookNode, xmlNamespace)
        {
            if(workbookNode.Name.ToLower() != "workbook")
            {
                throw new Exception("Unexpected content - not workbook");
            }

            //Note: [2015-10-28] Datasources presently don't return this information, so we need to make this workbook specific
            this.ContentUrl = workbookNode.Attributes["contentUrl"].Value;

            //Do we have tabs?
            this.ShowTabs = XmlHelper.SafeParseXmlAttribute_Bool(workbookNode, "showTabs", false);
        }


        /// <summary>
        /// Friendly text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Workbook: " + Name + "/" + ContentUrl + "/" + Id + ", Proj: " + ProjectId;
        }
    }
}
