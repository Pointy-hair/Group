using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;
using Traffk.Bal.Settings;
using RevolutionaryStuff.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Traffk.Bal.Email
{
    public class RawEmailer : IEmailer
    {
        private readonly IOptions<SmtpOptions> Options;

        public RawEmailer(IOptions<SmtpOptions> options)
        {
            Requires.NonNull(options, nameof(options));
            Options = options;
        }

        async Task IEmailer.SendEmailAsync(IEnumerable<MimeMessage> messages, Action<PreSendEventArgs> preSend, Action<PostSendEventArgs> postSend, object context)
        {
            var smtp = Options.Value;
            Requires.NonNull(smtp, nameof(smtp));
            Requires.Valid(smtp, nameof(smtp));

            var exceptions = new List<Exception>();
            using (var client = new SmtpClient())
            {
                client.LocalDomain = Stuff.Coalesce(smtp.LocalDomain, client.LocalDomain);
                await client.ConnectAsync(smtp.SmtpHost, smtp.SmtpPort, SecureSocketOptions.Auto).ConfigureAwait(false);
                await client.AuthenticateAsync(smtp.SmtpUser, smtp.SmtpPassword);
                int z = -1;
                foreach (var m in messages)
                {
                    ++z;
                    var pre = new PreSendEventArgs(m, context, z);
                    preSend?.Invoke(pre);
                    Exception sendException = null;
                    var sw = new Stopwatch();
                    if (!pre.Cancel)
                    {
                        try
                        {
                            sw.Start();
                            Debug.WriteLine($"Emailing [{m.Subject}] to {m.To.FirstOrDefault()?.Name}");
                            await client.SendAsync(m).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex);
                            sendException = ex;
                        }
                        sw.Stop();
                    }
                    if (postSend != null)
                    {
                        var post = new PostSendEventArgs(m, context, z, sw.Elapsed, pre.Cancel, sendException);
                        postSend(post);
                    }
                }
                if (postSend == null && exceptions.Count > 0)
                {
                    throw new AggregateException("issues sending email", exceptions);
                }
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
