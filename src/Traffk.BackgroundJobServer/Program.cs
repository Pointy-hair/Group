using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Traffk.BackgroundJobServer;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Data.Rdb.TraffkTenantShardManager;
using Traffk.Bal.Email;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;

namespace Traffk.BackgroundJobRunner
{
    public class Program : JobRunnerProgram
    {
        protected override void OnConfigureServices(IServiceCollection services)
        {
            base.OnConfigureServices(services);

            services.Configure<ActiveDirectoryApplicationIdentificationConfig>(Configuration.GetSection(ActiveDirectoryApplicationIdentificationConfig.ConfigSectionName));
            services.Configure<TenantManagementJobsRunner.Config>(Configuration.GetSection(TenantManagementJobsRunner.Config.ConfigSectionName));

            services.AddSingleton<IAsyncGetter<OpenIdConfiguration>>(new OpenIdConfigurationFinder(Configuration["AzureOpenIdConfigurationUrl"]));
            services.AddSingleton(Cache.DataCacher);
            services.AddScoped<ServiceClientCredentialFactory>();
            services.AddSingleton<IVault, Vault>();
            services.AddScoped<IOptions<SmtpOptions>, SmtpSettingsAdaptor>();
            services.AddScoped<ITrackingEmailer, TrackingEmailer>();
            services.AddScoped<ITenantJobs, TenantedJobRunner>();
            services.AddScoped<IGlobalJobs, GlobalJobRunner>();
            services.AddScoped<IDataSourceSyncJobs, DataSourceSyncRunner>();
            services.AddScoped<ITenantManagementJobs, TenantManagementJobsRunner>();
            services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
            services.AddDbContext<TraffkTenantShardManagerDbContext>((sp, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(TraffkTenantShardManagerDbContext.DefaultDatabaseConnectionStringName));
            }, ServiceLifetime.Scoped);
#if NET462
            services.Configure<EtlJobRunner.EtlJobRunnerConfig>(Configuration.GetSection(nameof(EtlJobRunner.EtlJobRunnerConfig)));
            services.AddScoped<IEtlJobs, EtlJobRunner>();
#endif
        }

#if true

        private void TraffkFtp(TraffkGlobalDbContext gdb, int tenantId, string filePattern, params string[] folderPaths)
            => Ftp(
                gdb,
                "traffk.ftptoday.com",
                Traffk.Bal.Services.Vault.CommonSecretUris.TraffkFtpTodayCredentialsUri,
                tenantId,
                filePattern,
                folderPaths
                );

        private void DeerwalkFtp(TraffkGlobalDbContext gdb, int tenantId, string filePattern, params string[] folderPaths)
            => Ftp(
                gdb,
                "ftp.deerwalk.com",
                Traffk.Bal.Services.Vault.CommonSecretUris.DeerwalkFtpCredentialsUri,
                tenantId,
                filePattern,
                folderPaths
                );

        private void Ftp(TraffkGlobalDbContext gdb, string ftpHost, string credentialsUri, int tenantId, string filePattern, params string[] folderPaths)
        {
            var ds = new Traffk.Bal.Data.Rdb.TraffkGlobal.DataSource
            {
                TenantId = tenantId,
                DataSourceSettings = new Traffk.Bal.Settings.DataSourceSettings
                {
                    FTP = new Traffk.Bal.Settings.DataSourceSettings.FtpSettings
                    {
                        CredentialsKeyUri = credentialsUri,
                        Hostname = ftpHost,
                        Port = 22,
                        FolderPaths = folderPaths,
                        FilePattern = filePattern,
                    },
                    DecompressItems = true,
                }
            };
            gdb.DataSources.Add(ds);
            gdb.SaveChanges();
            var rjm = (ITraffkRecurringJobManager)new TenantedBackgroundJobClient(gdb, new HardcodedTraffkTenantFinder(tenantId));
            rjm.Add(Hangfire.Common.Job.FromExpression<IDataSourceSyncJobs>(z => z.DataSourceFetchAsync(ds.DataSourceId)), Hangfire.Cron.Hourly());
        }

        public void FtpDS()
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(Configuration.GetConnectionString("TraffkGlobal"));
            GlobalJobFilters.Filters.Add(new TraffkJobFilterAttribute(Configuration));
            var gdb = this.ServiceProvider.GetRequiredService<TraffkGlobalDbContext>();
            DeerwalkFtp(gdb, 6, "traffk_uhaul_.+\\.zip", "/ftpdir/outgoing");
            DeerwalkFtp(gdb, 4, "(traffk_ethos)_.+\\.zip", "/ftpdir/outgoing");
            TraffkFtp(gdb, 4,  null, "/aetnapush", "/bbroussard@goempyrean.com", "/BKJolly@express-scripts.com", "/datatransfer@questdiagnostics.com");
            TraffkFtp(gdb, 5,  null, "/TopconData@blueshieldca.com");
            TraffkFtp(gdb, 6,  null, "/FileTransfer@azblue.com", "/markmcc@uhaul.com", "/U-HaulCarrum", "/U-HaulWorkday", "/U-HaulMetLife", "/PrideBroadspire");
            TraffkFtp(gdb, 7,  null, "/EnervestADP", "/EnervestBCBSTX", "/EnerVestHR");
            TraffkFtp(gdb, 8,  null, "/FileExchangeDispatch@anthem.com", "/PRIDE", "/PRIDEKP", "/BPA");
            TraffkFtp(gdb, 9,  null, "/PatriotNational");
            TraffkFtp(gdb, 11, null, "/TopconHealthComp", "/bradm@uhaul.com");
        }


        protected override Task OnGoAsync()
        {
//            FtpDS();
            return base.OnGoAsync();
        }


#if false
        private async Task GetEm(string folder)
        {
            var urls = new[] {
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist14_20140401_20150331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170401_20170430_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist14_20140401_20150331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist15_20150401_20160331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_20170501_20170531_20170607.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170501_20170531_20170608.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170501_20170531_20170607.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist16_20160401_20170331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist14_20140401_20150331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_20170401_20170430_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist15_20150401_20160331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170401_20170430_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist14_20140401_20150331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist14_20140401_20150331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_20170401_20170430_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist15_20150401_20160331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170601_20170630_20170710.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist16_20160401_20170331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist15_20150401_20160331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist14_20140401_20150331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_20170501_20170531_20170607.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_20170401_20170430_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170401_20170430_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist14_20140401_20150331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170601_20170630_20170709.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist15_20150401_20160331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170401_20170430_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_RX_20170601_20170630_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_RX_20170701_20170731_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/sfgtestfile.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_ELIG_20170501_20170531_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_ELIG_20170601_20170630_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist14_20140401_20150331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_RX_20170501_20170531_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist16_20160401_20170331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_20170601_20170630_20170709.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist16_20160401_20170331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_ELIG_20140101_20170430_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist14_20140401_20150331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist15_20150401_20160331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist15_20150401_20160331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist14_20140401_20150331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170401_20170430_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170501_20170531_20170607.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170401_20170430_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist16_20160401_20170331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist16_20160401_20170331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170501_20170531_20170608.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist14_20140401_20150331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist14_20140401_20150331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170601_20170630_20170710.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist16_20160401_20170331_20170512.ctl.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist16_20160401_20170331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist15_20150401_20160331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170401_20170430_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist15_20150401_20160331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_ELIG_20170701_20170731_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_MED_20170701_20170731_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_20170401_20170430_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist14_20140401_20150331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist15_20150401_20160331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170401_20170430_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_20170601_20170630_20170709.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist14_20140401_20150331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170401_20170430_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist15_20150401_20160331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_20170601_20170630_20170709.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_20170401_20170430_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist14_20140401_20150331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist14_20140401_20150331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist16_20160401_20170331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist16_20160401_20170331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_MED_20170501_20170531_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/sfgtestfile.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist14_20140401_20150331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist15_20150401_20160331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist16_20160401_20170331_20170512.txt.pgp",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MBRSHP_Hist15_20150401_20160331_20170512.ctl",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_MED_20170601_20170630_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist15_20150401_20160331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_MDCLM_Hist16_20160401_20170331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/FileExchangeDispatch@anthem.com/PrideIndust_TraffK_RXCLM_Hist16_20160401_20170331_20170512.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_RX_20140101_20170430_.txt",
                "abs://traffkhipprod.blob.core.windows.net/secure/Pride/85/20170806/085545/BPA/BPA_MED_20140101_20170430_.txt",            };
            var blobConfig = ServiceProvider.GetRequiredService<BSSC>().BlobConfigOptions.Value;
            var httpClientFactory = ServiceProvider.GetRequiredService<Utility.IHttpClientFactory>();
            int x = 0;
            foreach (var u in urls)
            {
                var uri = new Uri(u);
                if (uri.Scheme == BlobStorageServices.AzureBlobServiceProtocol)
                {
                    uri = BlobStorageServices.GetReadonlySharedAccessSignatureUrl(blobConfig, uri);
                }
                using (var client = httpClientFactory.Create())
                {
                    var parts = uri.LocalPath.Replace("/", "\\").Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    string fn = Path.Combine(folder, parts.Skip(5).Format("\\"));
                    Trace.WriteLine($"download {++x}/{urls.Length} from {u} to {fn}");
                    using (var st = await client.GetStreamAsync(uri))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fn));
                        using (var fst = File.Create(fn))
                        {
                            await st.CopyToAsync(fst);
                        }
                    }
                }
            }
        }

        public class BSSC
        {
            public IOptions<BlobStorageServices.Config> BlobConfigOptions;
            public BSSC(IOptions<BlobStorageServices.Config> blobConfigOptions)
            {
                BlobConfigOptions = blobConfigOptions;
            }
        }

        protected override Task OnGoAsync()
        {
            GetEm(@"C:\Users\jason\AppData\Local\Temp\pride\").ExecuteSynchronously();
            var key = File.ReadAllText(@"C:\Users\jason\Documents\Traffk\TraffkDataTransferPGPKeys - Copy (2).asc");
            using (var keyStream = StreamHelpers.Create(key, System.Text.ASCIIEncoding.ASCII))
            {
                using (var encryptedStream = File.OpenRead(@"C:\Users\jason\AppData\Local\Temp\fb70911d-0bc5-4a08-b31a-72de6c1ab817\DataSourceSyncRunner\1\655\fb70911d-0bc5-4a08-b31a-72de6c1ab817.113976156 (1).pgp"))
//                using (var encryptedStream = File.OpenRead(@"C:\Users\jason\Desktop\EthosEnergy_ELIG_07062017.txt.pgp"))
                {

                    BackgroundJobServer.EasyNetPGP.PgpEncryptorDecryptor.DecryptFile(
                        encryptedStream,
                        keyStream,
                        "1ne@WO3hree$OUR5ive^IX7even*IGHT9ine",
                        @"C:\Users\jason\Desktop\EthosEnergy_ELIG_07062017.txt");
                }
            }

            FtpDS();
            return base.OnGoAsync();
        }
        public void CreateRecurringTrace(string msg)
        {
            RJM.Add(Hangfire.Common.Job.FromExpression(() => System.Diagnostics.Trace.WriteLine(msg)), Cron.Minutely());
        }

        public void CRT(string msg)
        {
            RJM.Add(Hangfire.Common.Job.FromExpression<IDataSourceSyncJobs>(z => z.TraceMeAsync(msg)), Cron.Hourly());
        }

        public void CreateTenant(string tenantName)
        {
            if (tenantName == null) return BadRequest();
            var d = new TenantCreationDetails
            {
                AdminPassword = "1adminPassword",
                AdminUsername = "admin",
                TenantName = tenantName
            };
            Backgrounder.Enqueue<ITenantManagementJobs>(z => z.CreateTenant(d));
        }

        public void EtlZip()
        {
            Backgrounder.Enqueue<IEtlJobs>(z => z.ExecuteAsync(EtlPackages.ZipCodes, 256));
        }

        public void DownloadZipCodes()
        {
            var ds = new Traffk.Bal.Data.Rdb.TraffkGlobal.DataSource
            {
                TenantId = 3,
                DataSourceSettings = new Traffk.Bal.Settings.DataSourceSettings
                {
                    Web = new Traffk.Bal.Settings.DataSourceSettings.WebSettings
                    {
                        CredentialsKeyUri = Traffk.Bal.Services.Vault.CommonSecretUris.ZipCodesComCredentialsUri,
                        LoginPageConfig = new Traffk.Bal.Settings.DataSourceSettings.WebSettings.WebLoginPageConfig
                        {
                            LoginPage = new Uri("https://www.zip-codes.com/account_login.asp"),
                            UsernameFieldName = "loginUsername",
                            PasswordFieldName = "loginPassword"
                        },
                        DownloadUrls = new[]
                        {
                            new Uri("https://www.zip-codes.com/account_database.asp?type=csv&product=25"), //CSV Delux DB
                            new Uri("https://www.zip-codes.com/account_database.asp?type=csv&product=38"), //CSV Delux DB with Business
                            new Uri("https://www.zip-codes.com/account_database.asp?type=cs&product=89"), //CSV Zip9
                        }
                    }
                }
            };
            GDB.DataSources.Add(ds);
            GDB.SaveChanges();
            Backgrounder.Enqueue<IDataSourceSyncJobs>(z => z.DataSourceFetchAsync(ds.DataSourceId));
            //            RJM.Add(Hangfire.Common.Job.FromExpression<IDataSourceSyncJobs>(z => z.DataSourceFetchAsync(ds.DataSourceId)), Cron.Daily());
        }
#endif
#endif

        //BUGBUG: requires newtonsoft 9.0.1!  to prevent binder problem... yuck!  [Cannot get SerializationBinder because an ISerializationBinder was previously set.]
        //https://github.com/Azure/azure-sdk-for-net/issues/2552 -- requires newtonsoft 9.0.1!  to prevent binder problem... yuck!  [Cannot get SerializationBinder because an ISerializationBinder was previously set.]
        //https://stackoverflow.com/questions/35409905/how-do-i-create-an-azure-credential-that-will-give-access-to-the-websitemanageme
        //https://login.windows.net/traffk.onmicrosoft.com/.well-known/openid-configuration
        public static void Main(string[] args) => JobRunnerProgram.Main<Program>(args);
    }
}
