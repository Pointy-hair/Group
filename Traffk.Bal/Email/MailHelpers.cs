using MimeKit;
using MimeKit.Text;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Templates;

namespace Traffk.Bal.Email
{
    public static class MailHelpers
    {
        public const string ContactIdHeader = "x-Traffk-ContactId";
        public const string TopicHeader = "x-Traffk-Topic";
        public const string CampaignHeader = "x-Traffk-Campaign";
        public const string JobId = "x-Traffk-JobId";

        public static void Fill(this MimeMessage message, TemplateManager tm, MessageTemplate mt, ApplicationUser u, Contact c, object model)
        {
            Requires.NonNull(message, nameof(message));
            Requires.NonNull(tm, nameof(tm));
            Requires.NonNull(mt, nameof(mt));

            Template t;
            string s;

            var context = new Dictionary<string, object> { { "User", u }, { "Contact", c }, { "Model", model } };
            var indexerByName = new Dictionary<string, IDictionary<string, string>>();

            if (mt.SubjectTemplateId.HasValue)
            {
                t = tm.Finder.FindTemplateById(mt.SubjectTemplateId.Value);
                s = tm.Evaluate(t, context);
                message.Subject = s;
            }
            MimeEntity textPart = null;
            MimeEntity htmlPart = null;
            if (mt.TextBodyTemplateId.HasValue)
            {
                t = tm.Finder.FindTemplateById(mt.TextBodyTemplateId.Value);
                s = tm.Evaluate(t, context);
                textPart = new TextPart(TextFormat.Text) { Text = s };
            }
            if (mt.HtmlBodyTemplateId.HasValue)
            {
                t = tm.Finder.FindTemplateById(mt.HtmlBodyTemplateId.Value);
                s = tm.Evaluate(t, context);
                htmlPart = new TextPart(TextFormat.Html) { Text = s };
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
