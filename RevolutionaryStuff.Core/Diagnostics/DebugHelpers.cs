using RevolutionaryStuff.Core.Diagnostics.TraceListeners;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RevolutionaryStuff.Core.Diagnostics
{
    public static class DebugHelpers
    {
        private const string StartupMessage = "_Startup";
        private const string StartupDataMessage = "_StartupData";
        private const string ShutdownMessage = "_Shutdown";

        public static void TraceStartup()
        {
            var aEntry = Assembly.GetEntryAssembly();

            TraceStructured(StartupMessage);
            TraceStructured(StartupDataMessage, "DateTime.Now", DateTime.Now);
            TraceStructured(StartupDataMessage, "DateTime.UtcNow", DateTime.UtcNow);
            TraceStructured(StartupDataMessage, "Stuff.ApplicationFamily", Stuff.ApplicationFamily);
            TraceStructured(StartupDataMessage, "Stuff.ApplicationName", Stuff.ApplicationName);
            TraceStructured(StartupDataMessage, "Stuff.LocalApplicationDataFolder", Stuff.LocalApplicationDataFolder);
            TraceStructured(StartupDataMessage, "Environment.MachineName", Environment.MachineName);
            TraceStructured(StartupDataMessage, "RuntimeInformation.OSArchitecture", RuntimeInformation.OSArchitecture);
            TraceStructured(StartupDataMessage, "RuntimeInformation.OSDescription", RuntimeInformation.OSDescription);
            TraceStructured(StartupDataMessage, "RuntimeInformation.ProcessArchitecture", RuntimeInformation.ProcessArchitecture);
            TraceStructured(StartupDataMessage, "Environment.CurrentDirectory", Directory.GetCurrentDirectory());
            try
            {
                TraceStructured(StartupDataMessage, "Environment.CommandLine", Environment.GetCommandLineArgs().ToCsv());
            }
            catch (Exception)
            { }
            if (aEntry != null)
            {
                TraceStructured(StartupDataMessage, "EntryAssembly.FullName", aEntry.FullName);
            }
        }

        public static void TraceShutdown()
        {
            TraceStructured(ShutdownMessage);
        }

        public static void TraceStructured(string appMessageType, params object[] parts)
        {
            foreach (TraceListener tl in Trace.Listeners)
            {
                tl.TraceStructured(appMessageType, parts);
            }
        }

        public static void TraceError(Exception ex)
        {
            foreach (TraceListener tl in Trace.Listeners)
            {
                tl.TraceException(ex);
            }
        }

        public static void TraceWriteLine(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                format = string.Format(format, args);
            }
            Trace.WriteLine(format);
        }
    }
}
