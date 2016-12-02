using RevolutionaryStuff.Core.ApplicationParts;
using System.Diagnostics;

namespace RevolutionaryStuff.Core.Diagnostics.TraceListeners
{
    [NamedFactory(nameof(RotatingFileTraceListenerFactory))]
    public class RotatingFileTraceListenerFactory : ITraceListenerFactory
    {
        TraceListener ITraceListenerFactory.Construct() => new RotatingFileTraceListener();
    }
}
