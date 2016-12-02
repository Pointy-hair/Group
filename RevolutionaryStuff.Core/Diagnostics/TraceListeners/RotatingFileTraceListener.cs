using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RevolutionaryStuff.Core.Diagnostics.TraceListeners
{
    public class RotatingFileTraceListener : DispatchedTraceListener
    {
        public class Options
        {
            public string FileNameFormat { get; set; } = "App.{0:0000}.log";
            public int? RotateKBytes { get; set; } = 256;
            public int? RotateFileCount { get; set; } = 64;
            public string BaseLogFolder { get; set; }
            public string ListenerName { get; set; }
        }

        private static readonly Options DefaultOptions = new Options();

        private readonly RotatingFileTextWriter Writer;
        public readonly string LogFolder;

        public RotatingFileTraceListener(string baseLogFolder = null, string fileNameFormat = null, int? rotateKBytes = null, int? rotateFileCount = null, string name = null)
            : this(new Options { BaseLogFolder = baseLogFolder, FileNameFormat = fileNameFormat, RotateKBytes = rotateKBytes, RotateFileCount = rotateFileCount, ListenerName = name })
        { }

        public RotatingFileTraceListener(Options options)
            : base(true, (options??DefaultOptions).ListenerName ?? nameof(RotatingFileTraceListener))
        {
            options = options ?? DefaultOptions;

            LogFolder = options.BaseLogFolder ?? Path.Combine(Stuff.LocalApplicationDataFolder, "Logs");
            Writer = new RotatingFileTextWriter(
                Path.Combine(
                    LogFolder,
                    options.FileNameFormat ?? DefaultOptions.FileNameFormat
                    ),
                options.RotateKBytes.GetValueOrDefault(DefaultOptions.RotateKBytes.Value) * 1024,
                options.RotateFileCount.GetValueOrDefault(DefaultOptions.RotateFileCount.Value),
                false);
        }

        protected override void Dispose(bool disposing)
        {
            Writer.Dispose();
            base.Dispose(disposing);
        }

        private static void AddPrefix(List<object> parts, TraceMessageData tmd)
        {
            parts.AddRange(new object[] { tmd.MessageId, tmd.MessageCreatedAtUtc.ToSqlString(), tmd.ManagedThreadId, tmd.ThreadName, tmd.Type });
        }

        protected override void DispatchMessages(IList<TraceMessageData> messages)
        {
            var sb = new StringBuilder();
            var parts = new List<object>();
            bool flush = false;
            foreach (var tmd in messages)
            {
                parts.Clear();
                switch (tmd.Type)
                {
                    case TraceMessageTypes.Flush:
                        flush = true;
                        break;
                    case TraceMessageTypes.TraceException:
                        AddPrefix(parts, tmd);
                        var ex = tmd.Get<Exception>(0);
                        parts.AddRange(new object[] { ex.GetType().Name, ex.Message, ex.ToString() });
                        CSV.FormatLine(sb, parts);
                        break;
                    case TraceMessageTypes.TraceStructured:
                        AddPrefix(parts, tmd);
                        parts.Add(tmd.Get<string>(0));
                        parts.AddRange(tmd.Get<object[]>(1));
                        CSV.FormatLine(sb, parts);
                        break;
                    default:
                        AddPrefix(parts, tmd);
                        parts.AddRange(tmd.Args);
                        CSV.FormatLine(sb, parts);
                        break;
                }
            }
            Writer.Write(sb.ToString());
            if (flush)
            {
                Writer.Flush();
            }
        }
    }
}
