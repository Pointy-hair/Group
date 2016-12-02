using System.Collections.Generic;
using System.Diagnostics;

namespace RevolutionaryStuff.Core.Diagnostics.TraceListeners
{
    public class MungedTraceListener : DispatchedTraceListener
    {
        private readonly TraceListener Inner;
        public MungedTraceListener(TraceListener inner)
            : base(false, nameof(MungedTraceListener))
        {
            Requires.NonNull(inner, nameof(inner));
            Inner = inner;
        }

        protected override void OnDispose()
        {
            Inner.Dispose();
            base.OnDispose();
        }

        protected override void DispatchMessages(IList<TraceMessageData> messages)
        {
            foreach (var tmd in messages)
            {
                switch (tmd.Type)
                {
                    case TraceMessageTypes.Flush:
                        Inner.Flush();
                        return;
                }
                var msg = string.Format(
                    "[{1}, {0}, {2}, {3}] ",
                    tmd.ManagedThreadId,
                    tmd.ThreadName,
                    tmd.MessageCreatedAtUtc.ToLocalTime().ToSqlString(),
                    tmd.MessageId
                    );
                lock(Inner)
                {
                    Inner.Write(msg);
                    tmd.Invoke(Inner);
                }
            }
        }
    }
}
