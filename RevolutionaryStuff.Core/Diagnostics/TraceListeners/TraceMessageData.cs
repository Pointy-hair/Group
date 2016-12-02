using System;
using System.Diagnostics;

namespace RevolutionaryStuff.Core.Diagnostics.TraceListeners
{
    public enum TraceMessageTypes
    {
        Fail,
        Flush,
        Write,
        WriteLine,
        TraceEvent,
        TraceData,
        TraceException,
        TraceStructured,
    }

    public struct TraceMessageData
    {
        public TraceMessageTypes Type;
        public object[] Args;
        public string ThreadName;
        public int ManagedThreadId;
        public Int64 MessageId;
        public DateTime MessageCreatedAtUtc;

        public T Get<T>(int i)
        {
            return (T)Args[i];
        }

        public void Invoke(TraceListener inner)
        {
            var inner2 = inner as ITraceListener2;
            switch (Type)
            {
                case TraceMessageTypes.Flush:
                    inner.Flush();
                    break;
                case TraceMessageTypes.Fail:
                    inner.Fail(Get<string>(0), Get<string>(1));
                    break;
                case TraceMessageTypes.Write:
                    inner.Write(Get<object>(0), Get<string>(1));
                    break;
                case TraceMessageTypes.WriteLine:
                    inner.WriteLine(Get<object>(0), Get<string>(1));
                    break;
                case TraceMessageTypes.TraceEvent:
                    inner.TraceEvent(Get<TraceEventCache>(0), Get<string>(1), Get<TraceEventType>(2), Get<int>(3), Get<string>(4), Get<object[]>(5));
                    break;
                case TraceMessageTypes.TraceData:
                    inner.TraceData(Get<TraceEventCache>(0), Get<string>(1), Get<TraceEventType>(2), Get<int>(3), Get<object[]>(4));
                    break;
                case TraceMessageTypes.TraceException:
                    inner.TraceException(Get<Exception>(0));
                    break;
                case TraceMessageTypes.TraceStructured:
                    inner.TraceStructured(Get<string>(0), Get<object[]>(1));
                    break;
                default:
                    string msg = string.Format("Unexpected trace message type=[{0}]", Type);
                    Debug.Fail(msg);
                    break;
            }
        }
    }
}
