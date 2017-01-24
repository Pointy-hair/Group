using Traffk.Tableau.REST.RestRequests;

namespace Traffk.Tableau.REST
{
    public interface ITableauRestService
    {
        TableauServerSignIn Login { get; set; }
        TableauServerSignIn SignIn(TableauServerUrls onlineUrls, string userName, string password, TaskStatusLogs statusLog = null);
        DownloadProjectsList DownloadProjectsList(TableauServerUrls onlineUrls, TableauServerSignIn onlineLogin = null);
        DownloadViewsForSite DownloadViewsForSite(TableauServerUrls onlineUrls, TableauServerSignIn onlineLogin = null);
        DownloadViewsForSite DownloadViewsForSite();
        DownloadWorkbooksList DownloadWorkbooksList();
        DownloadWorkbooksList DownloadWorkbooksList(TableauServerUrls onlineUrls, TableauServerSignIn onlineLogin = null);
    }
}