using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

/// <summary>
/// Subclass of the WebClient object that allows use to set a larger/custom timout value so that longer downloads succeed
/// </summary>
public class TableauServerWebClient : HttpClient
{
    public readonly int WebRequestTimeout;
    public const int DefaultLongRequestTimeOutMs = 15 * 60 * 1000;  //15 minutes * 60 sec/minute * 1000 ms/sec

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="timeoutMs"></param>
    public TableauServerWebClient(int timeoutMs = DefaultLongRequestTimeOutMs)
    {
        this.WebRequestTimeout = timeoutMs;
    }

    /// <summary>
    /// Returns a Web Request object (used for download operations) with
    /// our specifically set timeout
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    protected WebRequest GetWebRequest(Uri address)
    {
        var request = WebRequest.Create(address);
        return request;
    }

    public HttpResponseMessage DownloadFile(string url, string tempFilepath)
    {
        try
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
        catch (Exception e)
        {
            throw;
        }
    }
}
