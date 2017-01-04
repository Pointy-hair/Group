using System.Threading.Tasks;

namespace Traffk.Tableau
{
    public interface ITrustedTicketGetter
    {
        Task<TrustedTicketGetter.TrustedTicketResult> Authorize();
        Task<string> GetTrustedTicket();
    }
}