using System;
using System.Net;
using System.IO;
using System.Net.Http;

/// <summary>
/// Subclass of the WebClient object that allows use to set a larger/custom timout value so that longer downloads succeed
/// </summary>
public class TableauServerWebClient : HttpClient
{
    public readonly int TimeoutInMilliseconds;
    public const int DefaultLongRequestTimeOutMs = 15 * 60 * 1000;  //15 minutes * 60 sec/minute * 1000 ms/sec

    public TableauServerWebClient(int timeoutInMilliseconds = DefaultLongRequestTimeOutMs)
    {
        this.TimeoutInMilliseconds = timeoutInMilliseconds;
    }

    public HttpResponseMessage DownloadFile(string url, string tempFilepath)
    {
        using (HttpResponseMessage response = GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result)
        {
            response.EnsureSuccessStatusCode();

            using (
                Stream contentStream = response.Content.ReadAsStreamAsync().Result,
                    fileStream = new FileStream(tempFilepath, FileMode.Create, FileAccess.Write, FileShare.None,
                        8192, true))
            {
                var buffer = new byte[8192];
                var isMoreToRead = true;

                do
                {
                    var read = contentStream.ReadAsync(buffer, 0, buffer.Length).Result;
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        fileStream.WriteAsync(buffer, 0, read);
                    }
                } while (isMoreToRead);
            }

            return response;
        }
    }
}
