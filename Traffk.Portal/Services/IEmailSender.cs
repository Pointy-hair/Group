using System.Threading.Tasks;

namespace TraffkPortal.Services
{
    public interface IEmailSender
    {
        Task SendEmailCommunicationAsync(string communicationPurpose, object model, string recipientAddress, string recipientName = null, long? contactId = null);
        Task SendEmailCommunicationAsync(int templateId, object model, string recipientAddress, string recipientName = null, long? contactId=null);
    }
}
