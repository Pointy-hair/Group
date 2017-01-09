using System.Threading.Tasks;
using Traffk.Bal.Communications;

namespace TraffkPortal.Services
{
    public interface IEmailSender
    {
        Task SendEmailCommunicationAsync(SystemCommunicationPurposes purpose, object model, string recipientAddress, string recipientName = null, long? contactId = null);
    }
}
