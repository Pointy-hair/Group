using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;
using Traffk.Utility;

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

        public DownloadDatasourceConnections(TableauServerUrls onlineUrls, TableauServerSignIn login, string datasourceId, IHttpClientFactory httpClientFactory)
            : base(onlineUrls, login, httpClientFactory)
        {
            DatasourceId = datasourceId;
        }

        public void ExecuteRequest()
        {
            var dsConnections = new List<SiteConnection>();

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = Urls.Url_DatasourceConnectionsList(Login, DatasourceId);
            var httpRequestMessage = CreateLoggedInRequest(urlQuery, HttpMethod.Get);

            Login.Logger.Information("Web request: " + urlQuery);
            var response = SendHttpRequest(httpRequestMessage);
            var xmlDoc = GetHttpResponseAsXml(response);

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
                    Login.Logger.Error("Error parsing workbook: " + itemXml.ToXmlNode());
                }
            } //end: foreach

            Connections_p = dsConnections;
        }
    }
}
