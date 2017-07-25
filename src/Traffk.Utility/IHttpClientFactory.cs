using System.Net.Http;

namespace Traffk.Utility
{
    public interface IHttpClientFactory
    {
        HttpClient Create(HttpMessageHandler handler = null, bool disposeHandler = false);
    }
}
