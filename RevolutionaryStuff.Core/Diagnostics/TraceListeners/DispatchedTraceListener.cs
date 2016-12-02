using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace RevolutionaryStuff.Core.Diagnostics.TraceListeners
{
    public abstract class DispatchedTraceListener : TraceListener, ITraceListener2
    {
        private readonly PeriodicAction Worker;
        private readonly ConcurrentQueue<TraceMessageData> MessageQueue = new ConcurrentQueue<TraceMessageData>();

        public DispatchedTraceListener(bool useBackgroundThread = true, string name = null)
            : base(name)
        {
            if (useBackgroundThread)
            {
                Worker = new PeriodicAction(BackgroundWriter, RevolutionaryStuffCoreOptions.Instance.BackgroundTraceListenerInterval);
            }
            if (name != null)
            {
                this.Name = name;
            }
        }

        private volatile bool Working;
        private void BackgroundWriter()
        {
            if (MessageQueue.Count == 0) return;
            try
            {
                Working = true;
                var messages = new List<TraceMessageData>(MessageQueue.Count);
                TraceMessageData md;
                while (MessageQueue.TryDequeue(out md))
                {
                    messages.Add(md);
                }
                DispatchMessages(messages);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Working = false;
            }
        }

        protected abstract void DispatchMessages(IList<TraceMessageData> messages);

        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        protected void WaitTillEmpty()
        {
            while (MessageQueue.Count > 0 || Working) Thread.Sleep(50);
        }

        protected virtual void OnDispose()
        { }

        protected override void Dispose(bool disposing)
        {
            WaitTillEmpty();
            if (Worker!=null)
            {
                Worker.Dispose();
            }
            OnDispose();
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            QueueMessage(TraceMessageTypes.Flush);
            WaitTillEmpty();
        }

        public override void Fail(string message)
        {
            Fail(message, null);
        }

        public override void Fail(string message, string detailMessage)
        {
            base.Fail(message, detailMessage);
            QueueMessage(TraceMessageTypes.Fail, message, detailMessage);
        }

        public override void Write(string message)
        {
            Write((object)message);
        }

        public override void Write(object o)
        {
            Write(o, null);
        }

        public override void Write(string message, string category)
        {
            Write((object)message, category);
        }

        public override void Write(object o, string category)
        {
            QueueMessage(TraceMessageTypes.Write, o, category);
        }

        public override void WriteLine(string message)
        {
            WriteLine((object)message);
        }

        public override void WriteLine(object o)
        {
            WriteLine(o, null);
        }

        public override void WriteLine(string message, string category)
        {
            WriteLine((object)message, category);
        }

        public override void WriteLine(object o, string category)
        {
            QueueMessage(TraceMessageTypes.WriteLine, o, category);
        }
/*
        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            QueueMessage(TraceMessageTypes.TraceTransfer, eventCache, source, id, message, relatedActivityId);
        }
        */
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEvent(eventCache, source, eventType, id, null);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEvent(eventCache, source, eventType, id, message, Empty.ObjectArray);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            QueueMessage(TraceMessageTypes.TraceEvent, eventCache, source, eventType, id, format, args);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            TraceData(eventCache, source, eventType, id, new object[] { data });
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            QueueMessage(TraceMessageTypes.TraceData, eventCache, source, eventType, id, data);
        }

        private Int64 MessageId;

        private void QueueMessage(TraceMessageTypes mt, params object[] args)
        {
            var tmd = new TraceMessageData
            {
                Type = mt,
                Args = args,
                ManagedThreadId = Thread.CurrentThread.ManagedThreadId,
                ThreadName = Thread.CurrentThread.Name,
                MessageId = Interlocked.Increment(ref MessageId),
                MessageCreatedAtUtc = DateTime.UtcNow
            };
            if (Worker == null)
            {
                DispatchMessages(new[] { tmd } );
            }
            else
            {
                MessageQueue.Enqueue(tmd);
            }
        }

        void ITraceListener2.TraceException(Exception ex)
        {
            QueueMessage(TraceMessageTypes.TraceException, ex);
        }

        void ITraceListener2.TraceStructured(string appMessageType, params object[] parts)
        {
            QueueMessage(TraceMessageTypes.TraceStructured, appMessageType, parts);
        }
    }
}
