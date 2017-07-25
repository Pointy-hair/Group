using System;
using Traffk.Utility;

namespace Traffk.Bal.Services
{
    public class CorrelationIdFactory : ICorrelationIdFactory
    {
        string ICorrelationIdFactory.Key { get; set; } = "X-Correlation-ID";

        string ICorrelationIdFactory.Create()
        {
            return "XCID" + Guid.NewGuid().ToString();
        }
    }
}
