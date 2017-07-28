using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Traffk.Utility;

namespace Traffk.Bal.Services
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public class Config
        {
            public const string ConfigSectionName = "HttpClientFactoryConfig";

            public IDictionary<string, string> HeaderValueByHeaderName { get; set; }
            public string UserAgentString { get; set; }
        }

        private readonly IOptions<Config> ConfigOptions;

        public HttpClientFactory(IOptions<Config> configOptions, ICorrelationIdFinder correlationIdFinder = null)
        {
            ConfigOptions = configOptions;

            if (correlationIdFinder != null)
            {
                var correlationId = correlationIdFinder.FindOrCreate();

                //Clear out existing correlationId
                ConfigOptions.Value.HeaderValueByHeaderName.Remove(correlationIdFinder.Key);

                //Add it to the HttpClient
                ConfigOptions.Value.HeaderValueByHeaderName.
                    Add(new KeyValuePair<string, string>(correlationIdFinder.Key, correlationId));
            }  
        }

        HttpClient IHttpClientFactory.Create(HttpMessageHandler handler, bool disposeHandler)
        {
            var config = ConfigOptions.Value;

            var c = handler==null ? new HttpClient() : new HttpClient(handler, disposeHandler);
            if (config.HeaderValueByHeaderName != null)
            {
                foreach (var kvp in config.HeaderValueByHeaderName)
                {
                    c.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                }
            }
            if (config.UserAgentString != null)
            {
                c.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse(config.UserAgentString));
            }

            return c;
        }
    }
}
