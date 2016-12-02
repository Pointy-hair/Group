using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RevolutionaryStuff.Core
{
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// The event's data
        /// </summary>
        public readonly T Data;

        [DebuggerStepThrough]
        public EventArgs(T data)
        {
            Data = data;
        }
    }

    public static class DelegateHelpers
    {
        private static bool AllowAllExceptions(Exception ex) { return true; }

        public static async Task<T> CallAndRetryOnFailureAsync<T>(Func<Task<T>> func, int? retryCount = 5, TimeSpan? backoffPeriod = null, Predicate<Exception> exceptionChecker = null)
        {
            exceptionChecker = exceptionChecker ?? AllowAllExceptions;
            backoffPeriod = backoffPeriod ?? TimeSpan.FromMilliseconds(250);
            int z = 0;
            for (;;)
            {
                try
                {
                    return await func();
                }
                catch (Exception ex)
                {
                    if (!exceptionChecker(ex)) throw;
                    if (z++ < retryCount.GetValueOrDefault(3))
                    {
                        var wait = Convert.ToInt32(backoffPeriod.Value.TotalMilliseconds * z);
                        Trace.WriteLine(string.Format("CallAndRetryOnFailure retry={0} wait={1}", z, TimeSpan.FromMilliseconds(wait)));
                        await (Task.Delay(wait));
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public static T CallAndRetryOnFailure<T>(Func<T> func, int? retryCount = 3, TimeSpan? backoffPeriod = null, Predicate<Exception> exceptionChecker = null)
        {
            exceptionChecker = exceptionChecker ?? AllowAllExceptions;
            backoffPeriod = backoffPeriod ?? TimeSpan.FromSeconds(2);
            int z = 0;
            for (;;)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    if (!exceptionChecker(ex)) throw;
                    if (z++ < retryCount.GetValueOrDefault(3))
                    {
                        var wait = Convert.ToInt32(backoffPeriod.Value.TotalMilliseconds * z);
                        Trace.WriteLine(string.Format("CallAndRetryOnFailure retry={0} wait={1}", z, TimeSpan.FromMilliseconds(wait)));
                        System.Threading.Thread.Sleep(wait);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public static T CallAndRetryOnFailure<T, E>(Func<T> func, int? retryCount = 3, TimeSpan? backoffPeriod = null) where E : Exception
        {
            Predicate<Exception> exceptionChecker = delegate (Exception ex)
            {
                return ex is E;
            };
            return CallAndRetryOnFailure(func, retryCount, backoffPeriod, exceptionChecker);
        }

        public static void Invoke(this EventHandler h, object sender, EventArgs e=null, bool throwException=false)
        {
            if (h == null) return;
            try
            {
                h(sender, e ?? EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw ex;
                }
                else
                {
#if DEBUG
                    Debug.WriteLine(ex);
#endif
                }
            }
        }
    }
}
