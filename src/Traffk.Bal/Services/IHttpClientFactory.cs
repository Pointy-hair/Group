using System.Net.Http;

namespace Traffk.Bal.Services
{
    public interface IHttpClientFactory
    {
        HttpClient Create(HttpMessageHandler handler = null, bool disposeHandler = false);
    }
}
