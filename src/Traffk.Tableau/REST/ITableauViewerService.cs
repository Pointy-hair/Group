using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public interface ITableauViewerService
    {
        TimeSpan ReportIndexCacheTimeout { get; }
        DownloadViewsForSite DownloadViewsForSite();
        byte[] DownloadPreviewImageForView(string workbookId, string viewId);
    }

    public class VisualId
    {
        [JsonProperty("worksheet")]
        public string Worksheet { get; set; }

        [JsonProperty("dashboard")]
        public string Dashboard { get; set; }

        [JsonProperty("flipboardZoneId")]
        public string FlipboardZoneId { get; set; }

        [JsonProperty("storyboard")]
        public string Storyboard { get; set; }

        [JsonProperty("storyPointId")]
        public string StoryPointId { get; set; }
    }

    public class GetUnderlyingDataOptions
    {
        [JsonProperty("ignoreAliases")]
        public bool IgnoreAliases { get; set; }

        [JsonProperty("ignoreSelection")]
        public bool IgnoreSelection { get; set; }

        [JsonProperty("includeAllColumns")]
        public bool IncludeAllColumns { get; set; }

        [JsonProperty("maxRows")]
        public int MaxRows { get; set; }

        [JsonProperty("visualId")]
        public VisualId VisualId { get; set; }

        /*
        Below are extra parameters I found from the .JS file from the function:
        https://traffk-my.sharepoint.com/personal/darren_traffk_com/Documents/tableau-2.1.0.js
        $addVisualIdToCommand: function WorksheetImpl$AddVisualIdToCommand(commandParameters)
         */
        [JsonProperty("worksheetName")]
        public string WorksheetName { get; set; }

        [JsonProperty("dashboardName")]
        public string DashboardName { get; set; }
    }

    public class CreatePdfOptions
    {
        [JsonProperty("worksheetName")]
        public string WorksheetName { get; set; }

        [JsonProperty("dashboardName")]
        public string DashboardName { get; set; }

        public int PixelWidth { get; set; }

        public int PixelHeight { get; set; }
    }

    public class DownloadPdfOptions
    {
        public DownloadPdfOptions(string workbookName, string viewName, string sessionid, Uri referrerUri, string tempFileKey, CookieCollection cookieCollection)
        {
            SessionId = sessionid;
            WorkbookName = workbookName;
            ViewName = viewName;
            ReferrerUri = referrerUri;
            TempFileKey = tempFileKey;
            CookiesJson = JsonConvert.SerializeObject(cookieCollection);
        }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("workbookName")]
        public string WorkbookName { get; set; }

        [JsonProperty("viewName")]
        public string ViewName { get; set; }

        [JsonProperty("tempFileKey")]
        public string TempFileKey { get; set; }

        [JsonProperty("referrerUri")]
        public Uri ReferrerUri { get; set; }

        [JsonProperty("cookiesJson")]
        public string CookiesJson { get; set; }

        public CookieContainer Cookies
        {
            get
            {
                var cookies = JsonConvert.DeserializeObject<Cookie[]>(CookiesJson);
                var cookieCollection = new CookieCollection();
                foreach (var cookie in cookies)
                {
                    cookieCollection.Add(cookie);
                }

                var cookieContainer = new CookieContainer();
                cookieContainer.Add(ReferrerUri, cookieCollection);

                return cookieContainer;
            }
        }
    }
}