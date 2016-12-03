using RevolutionaryStuff.Core;
using System.Threading.Tasks;
using Traffk.Bal.Email;
using MimeKit;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Templates;

namespace TraffkPortal.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IEmailer Emailer;
        private readonly CurrentContextServices Current;
        private readonly TraffkRdbContext DB;

        public AuthMessageSender(IEmailer emailer, CurrentContextServices current, TraffkRdbContext db)
        {
            Emailer = emailer;
            Current = current;
            DB = db;
        }

        Task ISmsSender.SendSmsAsync(string number, string message)
        {
            Requires.Text(number, nameof(number));
            Requires.Text(message, nameof(message));

            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

        private Task SendEmailAsync(MimeMessage message)
        {
            Requires.NonNull(message, nameof(message));
            Requires.Between(message.To.Count + message.Cc.Count + message.Bcc.Count, "message.Recipient.Count", 1);

            var ts = Current.Tenant.TenantSettings;
            if (message.From.Count == 0)
            {
                var appSettings = Current.Application.ApplicationSettings;
                var from = new MailboxAddress(
                    Stuff.CoalesceStrings(appSettings.EmailSenderName, ts.EmailSenderName),
                    Stuff.CoalesceStrings(appSettings.EmailSenderAddress, ts.EmailSenderAddress)
                    );
                message.From.Add(from);
            }
            return Emailer.SendEmailAsync(message);
        }

        async Task ISmsSender.SendSmsCommunicationAsync(string communicationPurpose, object model, string number)
        {
            var comm = await DB.GetSystemCommunication(communicationPurpose, SystemCommunication.CommunicationMediums.Sms, Current.Application.ApplicationId);
            Requires.NonNull(comm, nameof(comm));
            var message = new MimeMessage();          
            message.Fill(new TemplateManager(new DbTemplateFinder(DB, Current.TenantId), Current.Tenant.TenantSettings.ReusableValues), comm.MessageTemplate, Current.User, null, model);
            var body = ((TextPart)message.Body).Text;
            await ((ISmsSender)this).SendSmsAsync(number, body);
        }

        async Task IEmailSender.SendEmailCommunicationAsync(string communicationPurpose, object model, string recipientAddress, string recipientName, long? contactId)
        {
            var comm = await DB.GetSystemCommunication(communicationPurpose, SystemCommunication.CommunicationMediums.Email, Current.Application.ApplicationId);
            await SendEmailCommunicationAsync(comm, model, recipientAddress, recipientName, contactId);
        }

        async Task IEmailSender.SendEmailCommunicationAsync(int systemCommunicationId, object model, string recipientAddress, string recipientName, long? contactId)
        {
            var comm = await DB.GetSystemCommunication(systemCommunicationId);
            await SendEmailCommunicationAsync(comm, model, recipientAddress, recipientName, contactId);
        }

        private async Task SendEmailCommunicationAsync(SystemCommunication comm, object model, string recipientAddress, string recipientName, long? contactId)
        {
            Requires.NonNull(comm, nameof(comm));
            var message = new MimeMessage();
            if (contactId!=null)
            {
                message.Headers.Add(MailHelpers.ContactIdHeader, contactId.ToString());
            }
            message.To.Add(new MailboxAddress(recipientName ?? "", recipientAddress));
            message.Fill(new TemplateManager(new DbTemplateFinder(DB, Current.TenantId), Current.Tenant.TenantSettings.ReusableValues), comm.MessageTemplate, Current.User, null, model);
            await SendEmailAsync(message);
        }
    }
}
