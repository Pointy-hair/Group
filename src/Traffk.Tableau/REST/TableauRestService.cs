using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Traffk.Tableau.REST.RestRequests;
using RevolutionaryStuff.Core.Caching;
using System.Net;
using System.Net.Http;
using RevolutionaryStuff.Core;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Traffk.Tableau.REST.Models;
using Traffk.Tableau.VQL;

namespace Traffk.Tableau.REST
{
    public class TableauRestService : ITableauRestService
    {
        public TableauServerSignIn Login { get; set; }

        private readonly TableauSignInOptions Options;
        private readonly TableauServerUrls Urls;
        private readonly ICacher Cacher;
        private readonly ITrustedTicketGetter TrustedTicketGetter;
        TimeSpan ITableauRestService.ReportIndexCacheTimeout => Options.ReportIndexCacheTimeout;

        #region Constructors

        public TableauRestService(ITrustedTicketGetter trustedTicketGetter, IOptions<TableauSignInOptions> options, ICacher cacher=null)
        {
            Cacher = cacher ?? Cache.Passthrough;
            TrustedTicketGetter = trustedTicketGetter;
            Options = options.Value;
            Urls = TableauServerUrls.FromContentUrl(Options.RestApiUrl, 10);
            Login = SignIn(Urls, Options.Username, Options.Password);
        }

        #endregion

        public TableauServerSignIn SignIn(TableauServerUrls onlineUrls, string userName, string password, TaskStatusLogs statusLog = null)
        {
            return Cacher.FindOrCreate(
                Cache.CreateKey(onlineUrls.CacheKey, userName, password),
                key =>
                {
                    var l = new TableauServerSignIn(onlineUrls, userName, password, statusLog);
                    l.ExecuteRequest();
                    return new CacheEntry<TableauServerSignIn>(l, Options.LoginCacheTimeout);
                }).Value;
        }

        public DownloadProjectsList DownloadProjectsList()
        {
            var projects = new DownloadProjectsList(this.Urls, Login);
            projects.ExecuteRequest();
            return projects;
        }

        DownloadViewsForSite ITableauRestService.DownloadViewsForSite()
        {
            var views = new DownloadViewsForSite(Urls, Login);
            views.ExecuteRequest();
            return views;
        }

        DownloadWorkbooksList ITableauRestService.DownloadWorkbooksList()
        {
            var workbooksList = new DownloadWorkbooksList(Urls, Login);
            workbooksList.ExecuteRequest();
            return workbooksList;
        }

        SiteinfoSite ITableauRestService.CreateSite(string tenantName, out string url)
        {
            var addSite = new CreateSite(Urls, Login);
            var siteInfo = addSite.ExecuteRequest(tenantName);
            url = GetNewSiteUrl(siteInfo);
            return siteInfo;
        }

        private string GetNewSiteUrl(SiteinfoSite site)
        {
            return Options.RestApiUrl + "site/" + site.ContentUrl;
        }

        void ITableauRestService.AddUserToSite(string siteId, string userName)
        {
            var addUserToSite = new AddUserToSite(Urls, Login);
            addUserToSite.ExecuteRequest(siteId, userName);
        }

        ICollection<SiteWorkbook> ITableauRestService.DownloadWorkbooks(IEnumerable<SiteWorkbook> workbooksToDownload,
            string localSavePath,
            bool generateInfoFile)
        {
            var downloadWorkbooksRequest = new DownloadWorkbooks(Urls, Login, workbooksToDownload, localSavePath, generateInfoFile);
            var downloadedWorkbooks = downloadWorkbooksRequest.ExecuteRequest();
            return downloadedWorkbooks;
        }

        void ITableauRestService.UploadWorkbooks(string localUploadPath,
            string localPathTempWorkspace)
        {
            var uploadWorkbooksRequest = new UploadWorkbooks(Urls, Login, localUploadPath, localPathTempWorkspace);
            uploadWorkbooksRequest.ExecuteRequest();
        }

        byte[] ITableauRestService.DownloadPreviewImageForView(string workbookId, string viewId)
        {
            var downloadPreviewImage = new DownloadPreviewImageForView(Urls, Login);
            downloadPreviewImage.ExecuteRequest(workbookId, viewId);
            return downloadPreviewImage.PreviewImage;
        }

        private static readonly Regex CurrentWorkbookIdExpr = new Regex(@"\Wcurrent_workbook_id:\s*""(\d+)""", RegexOptions.Compiled);
        private static readonly Regex CurrentViewIdExpr = new Regex(@"\Wcurrent_view_id:\s*""(\d+)""", RegexOptions.Compiled);
        private static readonly Regex SheetIdExpr = new Regex(@"\WsheetId:\s*""(\d+)""", RegexOptions.Compiled);
        private static readonly Regex LastUpdatedAtExpr = new Regex(@"lastUpdatedAt\x22\x3A(\d+),", RegexOptions.Compiled);
        private static readonly Regex LayoutIdExpr = new Regex(@"""layoutId""\s*:\s*""(\d+)""", RegexOptions.Compiled);

        private class StickySessionKey
        {
            public string ToJson() => JsonConvert.SerializeObject(this);

            public object featureFlags { get; set; } = new object();
            public bool isAuthoring { get; set; }
            public bool isOfflineMode { get; set; }
            public string lastUpdatedAt { get; set; }
            public string viewId { get; set; }
            public string workbookId { get; set; }
        }

        async Task<UnderlyingDataTable> ITableauRestService.GetUnderlyingDataAsync(GetUnderlyingDataOptions options, string workbookName, string viewName)
        {
            var token = (await TrustedTicketGetter.AuthorizeAsync()).Token;
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true
            };
            using (var embedClient = new HttpClient(handler))
            {
                var uri = new Uri($"{Options.Url}/trusted/{token}/views/{workbookName}/{viewName}?:size=1610,31&:embed=y&:showVizHome=n&:jsdebug=y&:bootstrapWhenNotified=y&:tabs=n&:apiID=host0");
                embedClient.DefaultRequestHeaders.TryAddWithoutValidation("User Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
                embedClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                embedClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch, br");
                embedClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");
                var response = await embedClient.GetAsync(uri);
                IEnumerable<string> values;
                if (response.Headers.TryGetValues("X-Session-Id", out values))
                {
                    var sessionId = values.First();
                    var clientEmbedResponseString = await response.Content.ReadAsStringAsync();
                    var currentWorkbookId = CurrentWorkbookIdExpr.GetGroupValue(clientEmbedResponseString);
                    var currentViewId = CurrentViewIdExpr.GetGroupValue(clientEmbedResponseString);
                    var sheetId = SheetIdExpr.GetGroupValue(clientEmbedResponseString) ?? options.WorksheetName;
                    var lastUpdatedAt = LastUpdatedAtExpr.GetGroupValue(clientEmbedResponseString);
                    using (var bootstrapClient = new HttpClient(handler))
                    {
                        bootstrapClient.DefaultRequestHeaders.Referrer = uri;
                        bootstrapClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Tsi-Active-Tab", options.WorksheetName);
                        var d = new Dictionary<string, string>();
                        d["worksheetPortSize"] = "{\"w\":500,\"h\":440}";
                        d["dashboardPortSize"] = "{\"w\":500,\"h\":440}";
                        d["clientDimension"] = "{\"w\":500,\"h\":440}";
                        d["isBrowserRendering"] = "true";
                        d["browserRenderingThreshold"] = "100";
                        d["formatDataValueLocally"] = "false";
                        d["clientNum"] = "";
                        d["devicePixelRatio"] = "2";
                        d["clientRenderPixelLimit"] = "25000000";
                        d["sheet_id"] = options.WorksheetName;
                        d["showParams"] = "{\"revertType\":null,\"refresh\":false,\"checkpoint\":false,\"sheetName\":\"\",\"unknownParams\":\"\",\"layoutId\":\"\"}";
                        d["stickySessionKey"] = new StickySessionKey { lastUpdatedAt = lastUpdatedAt, viewId = currentViewId, workbookId = currentWorkbookId }.ToJson();
                        d["filterTileSize"] = "200";
                        d["workbookLocale"] = "";
                        d["locale"] = "en_US";
                        d["language"] = "en";
                        d["verboseMode"] = "true";
                        d[":session_feature_flags"] = "{}";
                        d["keychain_version"] = "1";
                        var content = new FormUrlEncodedContent(d);
                        response = await bootstrapClient.PostAsync(new Uri($"{Options.Url}/vizql/w/{workbookName}/v/{viewName}/bootstrapSession/sessions/{sessionId}"), content);
                        Stuff.Noop(response);
                        using (var postLoadOperationsClient = new HttpClient(handler))
                        {
                            /*
                            postLoadOperationsClient.DefaultRequestHeaders.Referrer = uri;
                            var bootstrapClientResponseString = await response.Content.ReadAsStringAsync();
                            var layoutId = LayoutIdExpr.GetGroupValue(bootstrapClientResponseString);
                            postLoadOperationsClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Tsi-Supports-Accepted", "true");
                            postLoadOperationsClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Tsi-Active-Tab", options.WorksheetName);
                            postLoadOperationsClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                            response = await postLoadOperationsClient.GetAsync(new Uri($"{Options.Url}/vizql/w/{workbookName}/v/{viewName}/performPostLoadOperations/sessions/{sessionId}/layouts/{layoutId}/?sheet_id={sheetId}"));
                            Stuff.Noop(response);
                            */
                            using (var getUnderlyingDataClient = new HttpClient(handler))
                            {
                                getUnderlyingDataClient.DefaultRequestHeaders.Referrer = uri;
                                getUnderlyingDataClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));
                                getUnderlyingDataClient.DefaultRequestHeaders.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/javascript"));
                                getUnderlyingDataClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Tsi-Supports-Accepted", "true");
                                getUnderlyingDataClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Tsi-Active-Tab", options.WorksheetName);
                                getUnderlyingDataClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                                var mc = new MultipartContent("form-data");
                                AddStringContent(mc, "dashboard", options.DashboardName);
                                AddStringContent(mc, "worksheet", options.WorksheetName);
                                AddStringContent(mc, "maxRows", options.MaxRows);
                                AddStringContent(mc, "ignoreAliases", options.IgnoreAliases);
                                AddStringContent(mc, "ignoreSelection", options.IgnoreSelection);
                                AddStringContent(mc, "includeAllColumns", options.IncludeAllColumns);
                                response = await getUnderlyingDataClient.PostAsync(new Uri($"{Options.Url}/vizql/w/{workbookName}/v/{viewName}/sessions/{sessionId}/commands/tabdoc/get-underlying-data"), mc);
                                Stuff.Noop(response);
                                var json = await response.Content.ReadAsStringAsync();
                                var vcra = JsonConvert.DeserializeObject<VqlCmdResponseWrapper>(json);
                                var dt = vcra?.vqlCmdResponse?.cmdResultList[0]?.commandReturn?.underlyingDataTable;
                                return dt;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void AddStringContent(MultipartContent m, string name, string value)
        {
            if (value!=null)
            {
                var sc = new StringContent(value);
                sc.Headers.Add(WebHelpers.HeaderStrings.ContentDisposition, $"form-data; name=\"{name}\"");
                m.Add(sc);
            }
        }

        private void AddStringContent(MultipartContent m, string name, int value)
            => AddStringContent(m, name, value.ToString());

        private void AddStringContent(MultipartContent m, string name, bool value)
            => AddStringContent(m, name, value ? "true" : "false");

        
    }
}
