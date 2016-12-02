using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core.Diagnostics.TraceListeners;
using System.Collections.Generic;
using System.Diagnostics;

namespace RevolutionaryStuff.Core.Diagnostics
{
    public class Tracer : BaseDisposable
    {
        public class Options
        {
            public string[] FactoryNames { get; set; } = new[] { "RotatingFileTraceListenerFactory" };
        }

        private static readonly Options DefaultOptions = new Options();

        public Tracer(Options options=null)
        {
            options = options ?? RevolutionaryStuffCoreOptions.Instance.Tracer ?? DefaultOptions;
            for (int z = 0; z < Trace.Listeners.Count; ++z)
            {
                var tl = Trace.Listeners[z];
                if (tl is DefaultTraceListener)
                {
                    tl = new MungedTraceListener(tl);
                    Trace.Listeners.RemoveAt(z);
                    Trace.Listeners.Insert(z, tl);
                }
            }
            foreach (var f in NamedFactoryAttribute.InstantiateFactories<ITraceListenerFactory>(options.FactoryNames))
            {
                var tl = f.Construct();
                Trace.Listeners.Add(tl);
            }
            DebugHelpers.TraceStartup();
        }

        protected override void OnDispose(bool disposing)
        {
            DebugHelpers.TraceShutdown();
            Trace.Flush();
            var listeners = new List<TraceListener>();
            foreach (TraceListener tl in Trace.Listeners)
            {
                listeners.Add(tl);
            }
            Trace.Listeners.Clear();
            Stuff.Dispose(listeners.ToArray());
            base.OnDispose(disposing);
        }
    }
}
