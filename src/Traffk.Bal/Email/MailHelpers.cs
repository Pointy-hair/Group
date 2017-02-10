using MimeKit;
using MimeKit.Text;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Communications;
using Traffk.Bal.Settings;
using System;

namespace Traffk.Bal.Email
{
    public static class MailHelpers
    {
        public static class HeaderNames
        {
            public const string ContactIdHeader = "x-Traffk-ContactId";
            public const string TopicHeader = "x-Traffk-Topic";
            public const string CampaignHeader = "x-Traffk-Campaign";
            public const string JobId = "x-Traffk-JobId";
        }

        #region Header Helpers

        public static Int64? ContactId(this MimeMessage m) => Parse.ParseNullableInt64(m.Headers[HeaderNames.ContactIdHeader]);
        public static void ContactId(this MimeMessage m, Int64 contactId) => m.Headers[HeaderNames.ContactIdHeader] = contactId.ToString();
        public static string Topic(this MimeMessage m) => m.Headers[HeaderNames.TopicHeader];
        public static void Topic(this MimeMessage m, string topic) => m.Headers[HeaderNames.TopicHeader] = topic;
        public static string Campaign(this MimeMessage m) => m.Headers[HeaderNames.CampaignHeader];
        public static void Campaign(this MimeMessage m, string campaign) => m.Headers[HeaderNames.CampaignHeader] = campaign;
        public static int? JobId(this MimeMessage m) => Parse.ParseNullableInt32(m.Headers[HeaderNames.JobId]);
        public static void JobId(this MimeMessage m, int jobId) => m.Headers[HeaderNames.JobId] = jobId.ToString();

        #endregion

        public static void Fill(
            this MimeMessage message,
            ICreativeSettingsFinder finder,
            ICollection<ReusableValue> reusableValues,
            Creative creative, 
            CommunicationTypes ct, 
            ApplicationUser u, 
            Contact c, 
            object model)
        {
            Requires.NonNull(message, nameof(message));
            Requires.NonNull(creative, nameof(creative));

            var filler = new CommunicationsFiller(finder, reusableValues);

            if (c != null)
            {
                message.ContactId(c.ContactId);
            }
            else if (u != null)
            {
                message.ContactId(u.ContactId);
            }


            var context = new Dictionary<string, object> { { "User", u }, { "Contact", c }, { "Model", model } };
            var compiledContext = new CommunicationsFiller.CompiledContext(context);
            var indexerByName = new Dictionary<string, IDictionary<string, string>>();

            MimeEntity textPart = null;
            MimeEntity htmlPart = null;
            switch (ct)
            {
                case CommunicationTypes.Email:
                    message.Subject = filler.Evaluate(creative, z=>z.EmailSubject, compiledContext);
                    textPart = new TextPart(TextFormat.Text) { Text = filler.Evaluate(creative, z => z.EmailTextBody, compiledContext) };
                    htmlPart = new TextPart(TextFormat.Html) { Text = filler.Evaluate(creative, z => z.EmailHtmlBody, compiledContext) };
                    break;
                case CommunicationTypes.Sms:
                    textPart = new TextPart(TextFormat.Text) { Text = filler.Evaluate(creative, z => z.TextMessageBody, compiledContext) };
                    break;
                default:
                    throw new UnexpectedSwitchValueException(ct);
            }

            if (textPart == null || htmlPart == null)
            {
                message.Body = textPart ?? htmlPart;
            }
            else
            {
                message.Body = new MultipartAlternative(textPart, htmlPart);
            }
        }

        public static IEnumerable<TextPart> FindHtmlParts(this MimeMessage mm)
        {
            var parts = new List<TextPart>();
            FindHtmlParts(mm.BodyParts, parts);
            return parts;
        }

        private static void FindHtmlParts(IEnumerable<MimeEntity> mes, ICollection<TextPart> parts)
        {
            foreach (var me in mes)
            {
                var tp = me as TextPart;
                if (tp != null && tp.IsHtml)
                {
                    parts.Add(tp);
                }
                else
                {
                    var mp = me as Multipart;
                    if (mp != null)
                    {
                        FindHtmlParts(mp, parts);
                    }
                }
            }
        }

        public static bool IsEmailAddressPartOfHostname(string emailAddress, params string[] hostnames)
        {
            if (string.IsNullOrEmpty(emailAddress)) return false;
            var pattern = $"\\w+@({hostnames.Join("|")})\\b";
            var expr = RegexHelpers.Create(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return expr.IsMatch(emailAddress);
        }

        public static Task SendEmailAsync(this IEmailer emailer, string subject, string messagePlain, string fromAddress, string toAddress, string fromName=null, string toName=null)
        {
            Requires.EmailAddress(fromAddress, nameof(fromAddress));
            Requires.EmailAddress(toAddress, nameof(toAddress));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName??"", fromAddress));
            message.To.Add(new MailboxAddress(toName ?? "", toAddress));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = messagePlain };
            return emailer.SendEmailAsync(new[] { message });
        }

        public static Task SendEmailAsync(
            this IEmailer emailer,
            MimeMessage message)
        {
            return emailer.SendEmailAsync(new[] { message });
        }
    }
}
