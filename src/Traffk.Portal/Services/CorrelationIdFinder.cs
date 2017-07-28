using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Traffk.Utility;

namespace Traffk.Portal.Services
{
    public class CorrelationIdFinder : ICorrelationIdFinder
    {
        private readonly ICorrelationIdFactory CorrelationIdFactory;
        private string Key_p;
        private readonly IHttpContextAccessor HttpContextAccessor;

        string ICorrelationIdFinder.Key { get => Key_p; set => Key_p = value; }

        public CorrelationIdFinder(ICorrelationIdFactory correlationIdFactory, IHttpContextAccessor httpContextAccessor)
        {
            CorrelationIdFactory = correlationIdFactory;
            Key_p = correlationIdFactory.Key;
            HttpContextAccessor = httpContextAccessor;
        }

        string ICorrelationIdFinder.FindOrCreate()
        {
            var httpContext = HttpContextAccessor.HttpContext;

            if (httpContext.Request.Headers.TryGetValue(CorrelationIdFactory.Key, out StringValues requestCorrelationId))
            {
                //CorrelationId already exists in HttpContext, which supersedes Client
                HttpContextAccessor.HttpContext.TraceIdentifier = requestCorrelationId;
                return requestCorrelationId;
            }

            //Create a new correlationId using the factory
            var correlationId = CorrelationIdFactory.Create();

            //Set the context trace identifier to the correlationId
            HttpContextAccessor.HttpContext.TraceIdentifier = correlationId;

            return correlationId;

        }
    }
}
