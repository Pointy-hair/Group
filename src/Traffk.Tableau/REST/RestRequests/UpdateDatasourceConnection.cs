using System.Net.Http;
using System.Text;
using System.Xml;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.RestRequests
{
    public class UpdateDatasourceConnection : TableauServerSignedInRequestBase
    {
        private readonly string TableauOperationDescription = "update datasource";
        public UpdateDatasourceConnection(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(onlineUrls, login)
        {

        }

        public void UpdateServerAddress(SiteDatasource datasourceToUpdate, SiteConnection connectionToUpdate, string serverAddress)
        {
            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb, XmlHelper.XmlSettingsForWebRequests);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("connection");
            xmlWriter.WriteAttributeString("serverAddress", serverAddress);
            xmlWriter.WriteEndElement();//</connection>
            xmlWriter.WriteEndElement(); // </tsRequest>
            xmlWriter.Dispose();

            var xmlText = sb.ToString();

            var urlQuery = Urls.UrlUpdateDatasourceConnection(Login, datasourceToUpdate, connectionToUpdate);
            var webRequest = CreateLoggedInRequest(urlQuery, HttpMethod.Put);

            SendHttpRequest(webRequest, xmlText);
        }
    }
}
