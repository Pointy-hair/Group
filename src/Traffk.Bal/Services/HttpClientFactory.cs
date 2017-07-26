using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using RevolutionaryStuff.Core;
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

        public HttpClientFactory(IOptions<Config> configOptions, 
             ICorrelationIdFactory correlationIdFactory,
             IHttpContextAccessor httpContextAccessor)
        {
            ConfigOptions = configOptions;

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext.Request.Headers.TryGetValue(correlationIdFactory.Key, out StringValues requestCorrelationId))
            {
                //CorrelationId already exists in HttpContext, which supersedes Client
                httpContextAccessor.HttpContext.TraceIdentifier = requestCorrelationId;
            }

            if (StringValues.IsNullOrEmpty(requestCorrelationId))
            {
                //Create a new correlationId using the factory
                var correlationId = correlationIdFactory.Create();
                //Add it to the HttpClient
                ConfigOptions.Value.HeaderValueByHeaderName.
                    Add(new KeyValuePair<string, string>(correlationIdFactory.Key, correlationId));
                //Set the context trace identifier to the correlationId
                httpContextAccessor.HttpContext.TraceIdentifier = correlationId;
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
