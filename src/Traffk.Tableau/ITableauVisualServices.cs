using System.Net.Http;
using System.Threading.Tasks;

namespace Traffk.Tableau
{
    public interface ITableauVisualServices
    {
        Task<string> GetTrustedTicket();

        Task<HttpContent> GetVisualization(string workbook, string view, string trustedTicket);
    }
}