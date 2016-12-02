using System;
using System.Diagnostics;

namespace RevolutionaryStuff.Core.Diagnostics.TraceListeners
{
    public interface ITraceListener2
    {
        void TraceException(Exception ex);
        void TraceStructured(string appMessageType, params object[] parts);
    }

    internal static class TraceListener2Extensions
    {
        public static void TraceException(this TraceListener tl, Exception ex)
        {
            var tl2 = tl as ITraceListener2;
            if (tl2 == null)
            {
                tl.WriteLine(ex);
            }
            else
            {
                tl2.TraceException(ex);
            }
        }

        public static void TraceStructured(this TraceListener tl, string appMessageType, params object[] parts)
        {
            var tl2 = tl as ITraceListener2;
            if (tl2 == null)
            {
                tl.WriteLine($"{appMessageType}, {CSV.FormatLine(parts, false)}");
            }
            else
            {
                tl2.TraceStructured(appMessageType, parts);
            }
        }
    }
}
