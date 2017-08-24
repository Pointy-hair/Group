using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Services;

namespace Traffk.Bal.ApplicationParts
{
    public partial class JobRunnerProgram
    {
        private class MyActivator : JobActivator
        {
            private readonly IServiceProvider ServiceProvider; 

            public MyActivator(JobRunnerProgram runner)
            {
                ServiceProvider = runner.ServiceProvider.CreateScope().ServiceProvider;
            }

            public override JobActivatorScope BeginScope(JobActivatorContext context)
                => new MyScope(ServiceProvider.CreateScope().ServiceProvider, context);

            internal JobActivatorScope BeginScope(int jobId)
                => new MyScope(ServiceProvider.CreateScope().ServiceProvider, jobId);

            public class MyScope : JobActivatorScope
            {
                private readonly IServiceProvider SP;

                private readonly JobInfo JobInfo = new JobInfo();

                internal MyScope(IServiceProvider sp, int jobId)
                    : this(sp, jobId, null)
                { }

                public MyScope(IServiceProvider sp, JobActivatorContext context)
                    : this(sp, int.Parse(context.BackgroundJob.Id), context.GetJobParameter<string>("RecurringJobId"))
                { }

                private MyScope(IServiceProvider sp, int jobId, string recurringJobId)
                {
                    SP = sp;

                    var gdb = sp.GetRequiredService<Data.Rdb.TraffkGlobal.TraffkGlobalDbContext>();
                    JobInfo.JobId = jobId;
                    var j = gdb.Job.Find(JobInfo.JobId);
                    if (j != null)
                    {
                        JobInfo.TenantId = j.TenantId;
                        JobInfo.ParentJobId = j.ParentJobId;
                        JobInfo.RecurringJobId = j.RecurringJobId;
                        JobInfo.ContactId = j.ContactId;
                    }
                    if (JobInfo.TenantId == null)
                    {
                        JobInfo.RecurringJobId = JobInfo.RecurringJobId ?? recurringJobId;
                        if (JobInfo.RecurringJobId != null)
                        {
                            var z = BackgroundJobs.TenantedBackgroundJobClient.ParseRecurringJobId(JobInfo.RecurringJobId);
                            JobInfo.TenantId = z.TenantId;
                            JobInfo.ContactId = z.ContactId;
                        }
                    }
                    ((MyJobInfoFinder)sp.GetRequiredService<IJobInfoFinder>()).JobInfo = JobInfo;
                    var tenantFinder = (MyTraffkTenantFinder)sp.GetRequiredService<ITraffkTenantFinder>();
                    tenantFinder.TenantId = JobInfo.TenantId;
                    if (JobInfo.ContactId != null)
                    {
                        var currentUser = (MyCurrentUser)sp.GetRequiredService<ICurrentUser>();
                        var tdb = sp.GetRequiredService<TraffkTenantModelDbContext>();
                        currentUser.User = tdb.Users.FirstOrDefault(u=>u.TenantId==JobInfo.TenantId && u.ContactId==JobInfo.ContactId);
                    }
                }

                public override object Resolve(Type type)
                    => SP.GetService(type);

                internal void Run()
                {
                    var gdb = SP.GetRequiredService<Data.Rdb.TraffkGlobal.TraffkGlobalDbContext>();
                    var job = gdb.Job.Find(JobInfo.JobId);
                    var id = InvocationData.CreateFromJson(job.InvocationData);
                    var service = SP.GetRequiredService(id.Type);
                    try
                    {
                        var parameterTypes = id.ParameterTypes;
                        var parameters = new List<object>();
                        var arguments = id.SerializedArguments;
                        for (int z = 0; z < arguments.Length; ++z)
                        {
                            var pt = parameterTypes[z];
                            object arg;
                            if (pt.IsEnum)
                            {
                                arg = Enum.Parse(pt, (string) arguments[z]);
                            }
                            else
                            {
                                arg = Convert.ChangeType(arguments[z], pt);
                            }
                            parameters.Add(arg);
                        }
                        var mi = id.Type.GetMethod(id.Method, parameterTypes);
                        var ret = mi.Invoke(service, parameters.ToArray());
                        if (ret is Task)
                        {
                            ((Task)ret).ExecuteSynchronously();
                        }
                    }
                    finally
                    {
                        Stuff.Dispose(service);
                    }
                }

                private class InvocationData
                {
                    private static Type LoadType(string typeName)
                    {
                        var t = Type.GetType(typeName);
                        if (t == null)
                        {
                            t = Type.GetType(typeName.LeftOf(","));
                        }
                        return t;
                    }

                    [JsonProperty("Type")]
                    public Type Type { get; set; }

                    [JsonProperty("Method")]
                    public string Method { get; set; }

                    [JsonProperty("ParameterTypes")]
                    public string ParameterTypesJson { get; set; }

                    [JsonIgnore]
                    public Type[] ParameterTypes
                        => JsonConvert.DeserializeObject<string[]>(ParameterTypesJson).ConvertAll(typeName => LoadType(typeName)).ToArray();

                    [JsonIgnore]
                    public object[] SerializedArguments
                        => JsonConvert.DeserializeObject<object[]>(ArgumentsJson);

                    [JsonProperty("Arguments")]
                    public string ArgumentsJson { get; set; }

                    public static InvocationData CreateFromJson(string json)
                        => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<InvocationData>(json);
                }
            }
        }
    }
}
