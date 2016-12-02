using Microsoft.Extensions.Configuration;
using System;

namespace RevolutionaryStuff.Core
{
    public class RevolutionaryStuffCoreOptions
    {
        internal static RevolutionaryStuffCoreOptions Instance
        {
            get
            {
                if (Instance_p == null) throw new NotNowException("This has not been initialized yet!");
                return Instance_p;
            }
        }
        private static RevolutionaryStuffCoreOptions Instance_p;

        public string ApplicationName { get; set; }

        public TimeSpan BackgroundTraceListenerInterval { get; set; } = TimeSpan.FromMilliseconds(1);

        public Diagnostics.TraceListeners.RotatingFileTraceListener.Options RotatingFileTraceListener { get; set; }

        public Diagnostics.Tracer.Options Tracer { get; set; }

        public static void Initialize(IConfiguration configuration)
        {
            Requires.NonNull(configuration, nameof(configuration));
            Initialize(configuration.GetSection(nameof(RevolutionaryStuffCoreOptions)));
        }

        private static bool InitializeCalled;
        public static void Initialize(IConfigurationSection section)
        {
            Requires.NonNull(section, nameof(section));
            Requires.SingleCall(ref InitializeCalled);
            Instance_p = section.Get<RevolutionaryStuffCoreOptions>();
        }
    }
}
