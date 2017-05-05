using RevolutionaryStuff.Core;
using Serilog;
using System.Threading;

namespace Traffk.Bal.ApplicationParts
{
    public abstract class BaseJobRunner : BaseDisposable
    {
        protected readonly ILogger Logger;
        protected readonly int InstanceId;

        private static int InstanceId_s;

        protected BaseJobRunner(ILogger logger)
        {
            InstanceId = Interlocked.Increment(ref InstanceId_s);
            //Push instanceid and type into context - thus into logs
            //Logger.ForContext
            //Log a message that says "Constructed"
        }

        protected override void OnDispose(bool disposing)
        {
            //Logger.something "Disposed"
            base.OnDispose(disposing);
        }
    }
}
