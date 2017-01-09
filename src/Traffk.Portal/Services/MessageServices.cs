﻿using RevolutionaryStuff.Core;
using System.Threading.Tasks;
using Traffk.Bal.Email;
using MimeKit;
using Traffk.Bal.Data.Rdb;
using TraffkPortal.Services.Sms;
using Microsoft.EntityFrameworkCore;
using System;
using Traffk.Bal.Communications;

namespace TraffkPortal.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly ITrackingEmailer Emailer;
        private readonly CurrentContextServices Current;
        private readonly TraffkRdbContext DB;
        private readonly ITwilioSmsSender TwilioSmsSender;

        public AuthMessageSender(ITrackingEmailer emailer, CurrentContextServices current, TraffkRdbContext db, ITwilioSmsSender twilioSmsSender)
        {
            Emailer = emailer;
            Current = current;
            DB = db;
            TwilioSmsSender = twilioSmsSender;
        }

        Task ISmsSender.SendSmsAsync(string number, string message)
            => TwilioSmsSender.SendSmsAsync(number, message);

        private Task SendEmailAsync(Creative creative, MimeMessage message)
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
            return Emailer.SendEmailAsync(creative, new[] { message });
        }

        private async Task<Tuple<Creative, MimeMessage>> CreateMessageAsync(SystemCommunicationPurposes purpose, object model)
        {
            int creativeId;
            Current.Application.ApplicationSettings.CreativeIdBySystemCommunicationPurpose.TryGetValue(purpose, out creativeId);
            Requires.Positive(creativeId, nameof(creativeId));
            var creative = await DB.Creatives.FindAsync(creativeId);
            Requires.NonNull(creative, nameof(creative));
            var message = new MimeMessage();
            message.Fill(DB, Current.Tenant.TenantSettings.ReusableValues, creative, CommunicationTypes.Email, Current.User, null, model);
            return Tuple.Create(creative, message);
        }

        async Task ISmsSender.SendSmsCommunicationAsync(SystemCommunicationPurposes purpose, object model, string number)
        {
            var res = await CreateMessageAsync(purpose, model);
            var body = ((TextPart)res.Item2.Body).Text;
            await ((ISmsSender)this).SendSmsAsync(number, body);
        }

        async Task IEmailSender.SendEmailCommunicationAsync(SystemCommunicationPurposes purpose, object model, string recipientAddress, string recipientName, long? contactId)
        {
            var res = await CreateMessageAsync(purpose, model);
            var creative = res.Item1;
            var message = res.Item2;
            if (contactId!=null)
            {
                message.ContactId(contactId.Value);
            }
            message.To.Add(new MailboxAddress(recipientName ?? "", recipientAddress));
            await SendEmailAsync(creative, message);
        }
    }
}
