using System.Collections.Generic;
using System.Diagnostics;

namespace RevolutionaryStuff.Core.Diagnostics.TraceListeners
{
    public sealed class BackgroundTraceListener : DispatchedTraceListener
    {
        private readonly TraceListener Inner;

        public BackgroundTraceListener(TraceListener inner, string name=null)
            : base(true, name)
        {
            Requires.NonNull(inner, nameof(inner));
            Inner = inner;
        }

        protected override void OnDispose()
        {
            Inner.Dispose();
            base.OnDispose();
        }

        public override string Name
        {
            get
            {
                return Inner.Name;
            }

            set
            {
                Inner.Name = value;
            }
        }

        protected override void DispatchMessages(IList<TraceMessageData> messages)
        {
            foreach (var tmd in messages)
            {
                tmd.Invoke(Inner);
            }
        }
    }
}
