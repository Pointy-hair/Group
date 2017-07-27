using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Dts.Runtime;
using RevolutionaryStuff.Core;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Services;
using Traffk.Utility;
using T = System.Threading.Tasks;

namespace Traffk.BackgroundJobServer
{
    public class EtlJobRunner : BaseJobRunner, IEtlJobs, IDTSEvents, IDTSLogging
    {
        private static readonly Assembly EtlPackageAssembly = typeof(EtlJobRunner).GetTypeInfo().Assembly;

        private readonly IOptions<EtlJobRunnerConfig> ConfigOptions;
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly IOptions<BlobStorageServices.Config> BlobConfig;
        private readonly Bal.Data.Rdb.TraffkTenantModel.TraffkTenantModelDbContext TtmDb;

        public EtlJobRunner(
            IOptions<EtlJobRunnerConfig> configOptions,
            Bal.Data.Rdb.TraffkTenantModel.TraffkTenantModelDbContext ttmDb,
            IOptions<BlobStorageServices.Config> blobConfig,
            IHttpClientFactory httpClientFactory,
            TraffkGlobalDbContext globalContext,
            IJobInfoFinder jobInfoFinder,
            ILogger logger)
            : base(globalContext, jobInfoFinder, logger)
        {
            ConfigOptions = configOptions;
            HttpClientFactory = httpClientFactory;
            TtmDb = ttmDb;
            BlobConfig = blobConfig;
        }

        bool IDTSLogging.Enabled => true;

        private bool ExecutePackageAsyncCalled = false;

        private async T.Task ExecutePackageAsync(string packageName, Action<Package> packageInit=null)
        {
            Requires.SingleCall(ref ExecutePackageAsyncCalled);

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

        private void SetDatasourceConnectionStringIfPresent(Package p, string dataSourceId, string connectionString)
        {
            foreach (var c in p.Connections)
            {
                if (c.ID == dataSourceId)
                {
                    c.ConnectionString = connectionString;
                    return;
                }
            }
        }

        private void SetParameterIfPresent(Package p, string parameterName, object parameterValue)
        {
            if (p.Parameters.Contains(parameterName))
            {
                p.Parameters[parameterName].Value = parameterValue;
            }
        }

        private void ConfigureCommonParameters(Package p)
        {
            SetDatasourceConnectionStringIfPresent(p, CommonDataSourceIds.TraffkGlobal, GlobalContext.Database.GetDbConnection().ConnectionString);
            SetDatasourceConnectionStringIfPresent(p, CommonDataSourceIds.ReferenceData, GlobalContext.Database.GetDbConnection().ConnectionString);
            SetParameterIfPresent(p, "WorkingFolderName", $"{TempFolderPath}work\\");
            if (JobInfo.TenantId.HasValue)
            {
                SetDatasourceConnectionStringIfPresent(p, CommonDataSourceIds.TraffkTenantModel, TtmDb.Database.GetDbConnection().ConnectionString);
                SetParameterIfPresent(p, "TenantId", JobInfo.TenantId.Value);
            }
        }

        public static class CommonDataSourceIds
        {
            public const string ReferenceData = "ReferenceData";
            public const string TraffkGlobal = "TraffkGlobal";
            public const string TraffkTenantModel = "TraffkTenantModel";
        }

        private string FetchFolder => $"{TempFolderPath}fetch\\";

        async T.Task IEtlJobs.ExecuteAsync(EtlPackages etlPackage, int dataSourceFetchId)
        {
            var fetch = await GlobalContext.DataSourceFetches.Include(z => z.DataSource).Include(z=>z.DataSourceFetchDataSourceFetchItems).SingleAsync(z => z.DataSource.TenantId == this.JobInfo.TenantId && z.DataSourceFetchId == dataSourceFetchId);

            Directory.CreateDirectory(FetchFolder);

            var fetchItems = fetch.DataSourceFetchDataSourceFetchItems.Where(z => z.DataSourceFetchItemTypeStringValue == DataSourceFetchItem.DataSourceFetchItemTypes.Original.ToString());

            Stuff.TaskWaitAllForEach(
                fetchItems,
                async fi => {
                    var fn = $"{FetchFolder}{fi.DataSourceFetchItemId}\\{fi.Name}";
                    Directory.CreateDirectory(Path.GetDirectoryName(fn));

                    var uri = new Uri(fi.Url);
                    if (uri.Scheme == BlobStorageServices.AzureBlobServiceProtocol)
                    {
                        uri = BlobStorageServices.GetReadonlySharedAccessSignatureUrl(BlobConfig, uri);
                    }
                    using (var client = HttpClientFactory.Create())
                    {
                        using (var st = await client.GetStreamAsync(uri))
                        {
                            using (var fst = File.Create(fn))
                            {
                                await st.CopyToAsync(fst);
                            }
                        }
                        if (MimeType.Application.Zip.DoesExtensionMatch(fn))
                        {
                            var unzipFolder = Path.Combine(Path.GetDirectoryName(fn), Path.GetFileNameWithoutExtension(fn));
                            ZipFile.ExtractToDirectory(fn, unzipFolder);
                        }
                    }
                });
            switch (etlPackage)
            {
                case EtlPackages.CmsGov:
                    await LoadCmsGovAsync(fetch);
                    return;
                case EtlPackages.InternationalClassificationDiseases:
                    await LoadInternationalClassificationDiseasesAsync(fetch);
                    break;
                case EtlPackages.NationalDrugCodes:
                    await LoadNationalDrugCodeAsync(fetch);
                    break;
                case EtlPackages.ZipCodes:
                    await LoadZipCodesAsync(fetch);
                    break;
                default:
                    throw new UnexpectedSwitchValueException(etlPackage);
            }
        }

        private T.Task LoadCmsGovAsync(DataSourceFetche fetch)
        {           
            return ExecutePackageAsync("Cmsgov.dtsx", p =>
            {
                ConfigureCommonParameters(p);
            });
        }

        private T.Task LoadZipCodesAsync(DataSourceFetche fetch)
        {
            return ExecutePackageAsync("ZipCodes.dtsx", p =>
            {
                ConfigureCommonParameters(p);
            });
        }

        private T.Task LoadInternationalClassificationDiseasesAsync(DataSourceFetche fetch)
        {
            return ExecutePackageAsync("InternationalClassificationDiseases.dtsx", p=> 
            {
                ConfigureCommonParameters(p);
            });
        }

        private T.Task LoadNationalDrugCodeAsync(DataSourceFetche fetch)
        {
            return ExecutePackageAsync("NationalDrugCode.dtsx", p =>
            {
                var c = ConfigOptions.Value?.NationalDrugCode;
                ConfigureCommonParameters(p);
                if (c?.DataUrl != null)
                {
                    p.Parameters["DataUrl"].Value = c.DataUrl.ToString();
                }
            });
        }

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