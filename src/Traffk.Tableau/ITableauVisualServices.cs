using System.Net.Http;
using System.Threading.Tasks;
using Traffk.Tableau.REST;
using Traffk.Tableau.VQL;

namespace Traffk.Tableau
{
    public interface ITableauVisualServices
    {
        Task<string> GetTrustedTicket();

        Task<HttpContent> GetVisualization(string workbook, string view, string trustedTicket);

        Task<UnderlyingDataTable> GetUnderlyingDataAsync(GetUnderlyingDataOptions options, string workbookName,
            string viewName);
    }
}