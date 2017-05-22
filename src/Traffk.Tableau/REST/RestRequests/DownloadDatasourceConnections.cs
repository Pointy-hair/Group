using System.Collections.Generic;
using System.Xml.Linq;

namespace Traffk.Tableau.REST.RestRequests
{
    /// <summary>
    /// Downloads the data connection in a published data source
    /// </summary>
    public class DownloadDatasourceConnections : TableauServerSignedInRequestBase
    {
        private readonly string DatasourceId;

        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        private List<SiteConnection> Connections_p;
        public ICollection<SiteConnection> Connections
        {
            get
            {
                var connections = Connections_p;
                if (connections == null) return null;
                return connections.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor: Call when we want to query the datasource on behalf of the currently logged in user
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public DownloadDatasourceConnections(TableauServerUrls onlineUrls, TableauServerSignIn login, string datasourceId)
            : base(onlineUrls, login)
        {
            DatasourceId = datasourceId;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public void ExecuteRequest()
        {
            var dsConnections = new List<SiteConnection>();

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = Urls.Url_DatasourceConnectionsList(Login, DatasourceId);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            Login.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebReponseLogErrors(webRequest, "get datasources's connections list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the workbook nodes
            //Get all the nodes
            var xDoc = xmlDoc.ToXDocument();
            var connections = xDoc.Root.Descendants(XName.Get("connection", XmlNamespace));

            //Get information for each of the data sources
            foreach (var itemXml in connections)
            {
                try
                {
                    var connection = new SiteConnection(itemXml.ToXmlNode());
                    dsConnections.Add(connection);
                }
                catch
                {
                    Login.StatusLog.AddError("Error parsing workbook: " + itemXml.ToXmlNode());
                }
            } //end: foreach

            Connections_p = dsConnections;
        }
    }
}
