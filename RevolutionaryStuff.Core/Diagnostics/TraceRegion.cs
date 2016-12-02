using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RevolutionaryStuff.Core.Diagnostics
{
    public class TraceRegion : BaseDisposable
    {
        private readonly string Name;
        private readonly Stopwatch Stopwatch;

        #region Constructors

        public TraceRegion([CallerMemberName] string name = null)
            : this(true, name)
        { }

        public TraceRegion(object name)
            : this(true, StringHelpers.ToString(name))
        { }

        public TraceRegion([CallerMemberName] string name = null, params object[] args)
            : this(true, name, args)
        { }

        public TraceRegion(bool timed, [CallerMemberName] string name = null, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                Name = string.Format(name, args);
            }
            else
            {
                Name = name;
            }
            if (!string.IsNullOrEmpty(Name))
            {
                var s = string.Format("{0} vvvvvvvvvvvvvvvvvvvvvvvv", Name);
                Trace.WriteLine(s);
            }
            Trace.Indent();
            if (timed) Stopwatch = Stopwatch.StartNew();
        }

        #endregion

        protected override void OnDispose(bool disposing)
        {
            string timing = null;
            if (Stopwatch != null)
            {
                Stopwatch.Stop();
                timing = string.Format(" duration={0}", Stopwatch.Elapsed);
            }
            Trace.Unindent();
            if (!string.IsNullOrEmpty(Name))
            {
                var s = string.Format("{0} ^^^^^^^^^^^^^^^^^^^^^^^^{1}", Name, timing);
                Trace.WriteLine(s);
            }
            base.OnDispose(disposing);
        }

        public static void Call(string name, Action a, bool catchAndPrintExceptions = true, bool throwExceptions = true, bool timer = true)
        {
            using (new TraceRegion(timer, name))
            {
                try
                {
                    a();
                }
                catch (Exception ex)
                {
                    if (catchAndPrintExceptions)
                    {
                        Debug.WriteLine(ex);
                    }
                    if (throwExceptions)
                    {
                        throw;
                    }
                }
            }
        }

        public static R Call<R>(string name, Func<R> a, bool catchAndPrintExceptions = true, bool timer = true)
        {
            using (new TraceRegion(timer, name))
            {
                try
                {
                    return a();
                }
                catch (Exception ex)
                {
                    if (catchAndPrintExceptions)
                    {
                        Trace.WriteLine(ex);
                    }
                    throw;
                }
            }
        }
    }
}
