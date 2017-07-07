﻿using System.IO;
using System.Net.Http;
using RevolutionaryStuff.Core;
using Traffk.Tableau.REST.Helpers;

namespace Traffk.Tableau.REST.RestRequests
{
    public class DownloadPreviewImageForView : TableauServerSignedInRequestBase
    {
        public byte[] PreviewImage {
            get { return _previewImage; }
        }

        private readonly TableauServerUrls urls;
        private readonly string xmlNamespace;
        private byte[] _previewImage;

        public DownloadPreviewImageForView(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            urls = onlineUrls;
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline", "http://tableau.com/api");
            xmlNamespace = nsManager.LookupNamespace("iwsOnline");
        }

        public void ExecuteRequest(string workbookId, string viewId)
        {
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = urls.UrlDownloadPreviewImageForView(Login, workbookId, viewId);
            var request = CreateLoggedInRequest(urlQuery, HttpMethod.Get);

            //Login.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = SendHttpRequest(request);
            var responseStream = response.GetResponseStreamAsync().ExecuteSynchronously();

            using (MemoryStream ms = new MemoryStream())
            {
                responseStream.CopyTo(ms);
                _previewImage = ms.ToArray();
            }
        }
    }
}
