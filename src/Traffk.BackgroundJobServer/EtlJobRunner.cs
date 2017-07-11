#if NET462
using Microsoft.SqlServer.Dts.Runtime;
using RevolutionaryStuff.Core;
using Serilog;
using System.IO;
using System.Reflection;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Serilog.Events;
using T = System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Traffk.BackgroundJobServer
{
    public class EtlJobRunner : BaseJobRunner, IEtlJobs, IDTSEvents, IDTSLogging
    {
        private static readonly Assembly EtlPackageAssembly = typeof(EtlJobRunner).GetTypeInfo().Assembly;

        private readonly IOptions<EtlJobRunnerConfig> ConfigOptions;

        public EtlJobRunner(
            IOptions<EtlJobRunnerConfig> configOptions,
            TraffkGlobalDbContext globalContext,
            IJobInfoFinder jobInfoFinder,
            ILogger logger)
            : base(globalContext, jobInfoFinder, logger)
        {
            ConfigOptions = configOptions;
        }

        bool IDTSLogging.Enabled => true;

        private bool ExecuteAsyncCalled = false;

        public async T.Task ExecuteAsync(string packageName, Action<Package> packageInit=null)
        {
            Requires.SingleCall(ref ExecuteAsyncCalled);

            var dtsxPath = Path.Combine(TempFolderPath, packageName);
            try
            {
                using (var rst = ResourceHelpers.GetEmbeddedResourceAsStream(EtlPackageAssembly, packageName))
                {
                    using (var tst = File.Create(dtsxPath))
                    {
                        await rst.CopyToAsync(tst);
                    }
                }
                var app = new Application();
                var package = app.LoadPackage(dtsxPath, this);
                packageInit?.Invoke(package);
                var res = package.Execute(null, null, this, this, null);
            }
            finally
            {
                Stuff.FileTryDelete(dtsxPath);
            }
        }

        #region IDTSEvents

        private void LogDtsEvent(LogEventLevel eventLevel, string template, object[] propertyValues, [CallerMemberName] string caller=null)
        {
            var pvs = new List<object>(propertyValues);
            pvs.Insert(0, "IDTSEvents." + caller);
            pvs.Insert(0, this.GetType().Name);
            Logger.Write(eventLevel, "Class={@className} Api={@api} "+template, propertyValues);
        }

        private void LogDtsEventInformation(string template, object[] propertyValues, [CallerMemberName] string caller = null)
            => LogDtsEvent(LogEventLevel.Information, template, propertyValues, caller);

        void IDTSEvents.OnBreakpointHit(IDTSBreakpointSite breakpointSite, BreakpointTarget breakpointTarget)
        {
            LogDtsEventInformation(
                "breakpointSite:{@breakpointSite} breakpointTarget:{@breakpointTarget}", 
                new object[] { breakpointSite, breakpointTarget });
        }

        void IDTSEvents.OnCustomEvent(TaskHost taskHost, string eventName, string eventText, ref object[] arguments, string subComponent, ref bool fireAgain)
        {
            LogDtsEventInformation(
                "taskHost:{@taskHost} eventName:{eventName} eventText={eventText} subComponent={subComponent}", 
                new object[] { taskHost, eventName, eventText, subComponent });
        }

        bool IDTSEvents.OnError(DtsObject source, int errorCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
        {
            return false;
        }

        void IDTSEvents.OnExecutionStatusChanged(Executable exec, DTSExecStatus newStatus, ref bool fireAgain)
        {
        }

        void IDTSEvents.OnInformation(DtsObject source, int informationCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError, ref bool fireAgain)
        {
            LogDtsEventInformation(
                "source:{@source} informationCode:{informationCode} subComponent={subComponent} description={description} ", 
                new object[] { source, informationCode, subComponent, description });
        }

        void IDTSEvents.OnPostExecute(Executable exec, ref bool fireAgain)
        {
            throw new NotImplementedException();
        }

        void IDTSEvents.OnPostValidate(Executable exec, ref bool fireAgain)
        {
            throw new NotImplementedException();
        }

        void IDTSEvents.OnPreExecute(Executable exec, ref bool fireAgain)
        {
            throw new NotImplementedException();
        }

        void IDTSEvents.OnPreValidate(Executable exec, ref bool fireAgain)
        { }

        void IDTSEvents.OnProgress(TaskHost taskHost, string progressDescription, int percentComplete, int progressCountLow, int progressCountHigh, string subComponent, ref bool fireAgain)
        { }

        bool IDTSEvents.OnQueryCancel()
        {
            return false;
        }

        void IDTSEvents.OnTaskFailed(TaskHost taskHost)
        {
            LogDtsEvent(
               LogEventLevel.Error,
               "taskHost:{@taskHost}",
               new object[] { taskHost});

        }

        void IDTSEvents.OnVariableValueChanged(DtsContainer DtsContainer, Variable variable, ref bool fireAgain)
        { }

        void IDTSEvents.OnWarning(DtsObject source, int warningCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
        {
            LogDtsEvent(
                LogEventLevel.Warning,
                "source:{@source} warningCode:{warningCode} subComponent={subComponent} description={description} ",
                new object[] { source, warningCode, subComponent, description });
        }

        void IDTSLogging.Log(string eventName, string computerName, string operatorName, string sourceName, string sourceGuid, string executionGuid, string messageText, DateTime startTime, DateTime endTime, int dataCode, ref byte[] dataBytes)
        {
            throw new NotImplementedException();
        }

        bool[] IDTSLogging.GetFilterStatus(ref string[] eventNames)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEtlJobs

        private void SetParameterIfPresent(Package p, string parameterName, object parameterValue)
        {
            if (p.Parameters.Contains(parameterName))
            {
                p.Parameters[parameterName].Value = parameterValue;
            }
        }

        private void ConfigureCommonParameters(Package p)
        {
            SetParameterIfPresent(p, "WorkingFolderName", $"{TempFolderPath}work");
        }

        T.Task IEtlJobs.LoadCmsGovAsync()
        {
            return ExecuteAsync("Cmsgov.dtsx", p =>
            {
                ConfigureCommonParameters(p);
            });
        }

        T.Task IEtlJobs.LoadInternationalClassificationDiseasesAsync()
        {
            return ExecuteAsync("InternationalClassificationDiseases.dtsx", p=> 
            {
                ConfigureCommonParameters(p);
            });
        }

        T.Task IEtlJobs.LoadNationalDrugCodeAsync()
        {
            return ExecuteAsync("NationalDrugCode.dtsx", p =>
            {
                var c = ConfigOptions.Value?.NationalDrugCode;
                ConfigureCommonParameters(p);
                if (c?.DataUrl != null)
                {
                    p.Parameters["DataUrl"].Value = c.DataUrl.ToString();
                }
            });
        }

        #endregion


        public class EtlJobRunnerConfig
        {
            public class NationalDrugCodeConfig
            {
                public Uri DataUrl { get; set; }
            }

            public NationalDrugCodeConfig NationalDrugCode { get; set; }
        }

    }
}
#endif