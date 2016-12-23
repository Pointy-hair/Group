﻿using RevolutionaryStuff.Core.Caching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RevolutionaryStuff.Core
{
    public static class Stuff
    {
        public static readonly Assembly ThisAssembly;

        static Stuff()
        {
            ThisAssembly = typeof(Stuff).GetTypeInfo().Assembly;
            var a = Assembly.GetEntryAssembly();
            var info = a?.GetInfo();
            ApplicationName = StringHelpers.Coalesce(info?.Title, a?.GetName().Name, "Unnamed");
            ApplicationFamily = StringHelpers.Coalesce(info?.Product, info?.Company, ApplicationName);
        }

        public const string BaseRsllcUrn = "urn:www.revolutionarystuff.com";

        public static readonly CultureInfo CultureUS = new CultureInfo("en-US");

        public static readonly string ApplicationName;

        public static readonly string ApplicationFamily;

        public static readonly Guid ApplicationInstanceId = Guid.NewGuid();

        /// <summary>
        /// Random number generator with a fixed seed.  Useful for testing.
        /// </summary>
        public static readonly Random RandomWithFixedSeed = new Random(19740409);

        /// <summary>
        /// Random number generator with a random seed value.
        /// </summary>
        public static readonly Random RandomWithRandomSeed = new Random(Crypto.Salt.RandomInteger);

        /// <summary>
        /// Instance of a random number generator
        /// </summary>
        public static readonly Random Random = RandomWithRandomSeed;

        /// <summary>
        /// Does nothing.  It is simply used as a line where one can set breakpoints
        /// </summary>
        /// <param name="args">Pass in parameters if you don't want them compiled out</param>
        [Conditional("DEBUG")]
        public static void Noop(params object[] args)
        {
        }

        /// <summary>
        /// Returns the first non-null value in the input
        /// </summary>
        /// <param name="vals">The list of options</param>
        /// <returns>The first non-null value.  If all are null, null is returned</returns>
        public static T Coalesce<T>(params T[] vals)
        {
            if (vals != null)
            {
                for (int x = 0; x < vals.Length; ++x)
                {
                    if (vals[x] != null) return vals[x];
                }
            }
            return default(T);
        }

        public static string CoalesceStrings(params string[] vals)
        {
            for (int x = 0; x < vals.Length; ++x)
            {
                if (vals[x] != null && vals[x].Length > 0) return vals[x];
            }
            return null;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        public static T Min<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) < 0 ? a : b;
        }

        public static T Max<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) < 0 ? b : a;
        }

        /// <summary>
        /// Convert a tickcount that was created in this windows session to a date time
        /// </summary>
        /// <param name="TickCount">The tickcount</param>
        /// <returns>The datetime when this happened</returns>
        public static DateTime TickCount2DateTime(int tickCount)
        {
            DateTime n = DateTime.Now;
            int tc = Environment.TickCount;
            return n.AddMilliseconds(tickCount - tc);
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            var ret = new List<T>();
            foreach (var v in Enum.GetValues(typeof(T)))
            {
                ret.Add((T)v);
            }
            return ret;
        }

        /// <summary>
        /// Is the enum that is marked with the FlagsAttribute equal to the passed in value
        /// </summary>
        /// <param name="Flags">The flags enum</param>
        /// <param name="Val">The value we are testing against</param>
        /// <returns>True if the test and the flag are the same, else false</returns>
        public static bool FlagEq(Enum Flags, Enum Val)
        {
            var flags = (long)Convert.ChangeType(Flags, typeof(long), null);
            var val = (long)Convert.ChangeType(Val, typeof(long), null);
            return val == (flags & val);
        }

        /// <summary>
        /// Dispose an object if it has an IDisposable interface
        /// </summary>
        /// <param name="o">The object</param>
        public static void Dispose(params object[] os)
        {
            if (os == null) return;
            foreach (object o in os)
            {
                var d = o as IDisposable;
                if (d == null) return;
                try
                {
                    d.Dispose();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Get an embedded resource as a stream
        /// </summary>
        /// <param name="name">The unqualified name of the resource</param>
        /// <param name="a">The assembly that houses the resource, if null, uses the caller</param>
        /// <returns>The stream, else null</returns>
        public static Stream GetEmbeddedResourceAsStream(this Assembly a, string name)
        {
            Requires.NonNull(a, nameof(a));
            if (null == name) return null;
            string[] streamNames = a.GetManifestResourceNames();
            name = name.ToLower();
            if (Array.IndexOf(streamNames, name) == -1)
            {
                foreach (string streamName in streamNames)
                {
                    if (streamName.EndsWith(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        int i = name.Length + 1;
                        if (streamName.Length < i || streamName[streamName.Length - i] == '.')
                        {
                            name = streamName;
                            break;
                        }
                    }
                }
            }
            return a.GetManifestResourceStream(name);
        }

        /// <summary>
        /// Get an embedded resource as a string
        /// </summary>
        /// <param name="name">The unqualified name of the resource</param>
        /// <param name="a">The assembly that houses the resource, if null, uses the caller</param>
        /// <returns>The string, else null</returns>
        public static string GetEmbeddedResourceAsString(this Assembly a, string name)
        {
            using (var st = a.GetEmbeddedResourceAsStream(name))
            {
                if (st == null) return null;
                return st.ReadToEnd();
            }
        }

        public static void InvokeEvent(EventHandler h, object sender, EventArgs e=null, bool throwException=false)
        {
            Debug.Assert(!(sender is EventArgs), "Probably a programming bug in porting the old RSLLC.JBT");
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
                    Debug.WriteLine(ex);
                }
            }
        }

        private static bool AllowAllExceptions(Exception ex) { return true; }

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

        /// <summary>
        /// Enter a critical section and act, or if someone is already inside, wait, but don't execute
        /// </summary>
        /// <param name="actor">The action to take</param>
        /// <param name="locker">A lock</param>
        public static void SingleActor(Action actor, object locker=null)
        {
            Requires.NonNull(actor, nameof(actor));
            locker = locker ?? actor;
            if (Monitor.TryEnter(locker))
            {
                try
                {
                    actor();
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }
            else
            {
                lock (locker) { }
            }
        }

        public static TResult ExecuteSynchronously<TResult>(this Task<TResult> task)
        {
            var t = Task.Run(async () => await task);
            t.Wait();
            if (t.IsFaulted) throw task.Exception;
            return t.Result;
        }

        public static void ExecuteSynchronously(this Task task)
        {
            var t = Task.Run(async () => await task);
            t.Wait();
            if (t.IsFaulted) throw task.Exception;
        }

        private static readonly ICache<string, string> GetPathFromSerializedPathCache = Cache.CreateSynchronized<string, string>();

        public static string GetPathFromSerializedPath(Type t, string serializedPath)
        {
            return GetPathFromSerializedPathCache.Do(Cache.CreateKey(t, serializedPath), () => {
                if (serializedPath == null) return null;
                string left = serializedPath.LeftOf(".");
                string right = StringHelpers.TrimOrNull(serializedPath.RightOf("."));

                foreach (var pi in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (pi.GetCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>() != null) continue;
                    var jpn = pi.GetCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>();
                    if ((jpn == null && pi.Name == left) || jpn.PropertyName == left)
                    {
                        left = pi.Name;
                        if (right == null) return left;
                        right = GetPathFromSerializedPath(pi.PropertyType, right);
                        if (right == null) return null;
                        return left + "." + right;
                    }
                }
                return null;
            });
        }
    }
}