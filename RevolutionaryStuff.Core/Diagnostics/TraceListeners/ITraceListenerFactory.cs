using System.Diagnostics;

namespace RevolutionaryStuff.Core.Diagnostics.TraceListeners
{
    public interface ITraceListenerFactory
    {
        TraceListener Construct();
    }
}
