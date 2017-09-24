using Microsoft.Azure.Management.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Traffk.Bal;
using Traffk.Bal.ApplicationParts;
using Traffk.Bal.BackgroundJobs;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Services;
using RevolutionaryStuff.Core.ApplicationParts;
using System.Collections;
using System.Linq;
using Traffk.Bal.Permissions;
using Microsoft.AspNetCore.Identity;
using Hangfire;
using Traffk.Bal.Data.Rdb.TraffkTenantShards;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Data.Rdb.TraffkTenantShardManager;

namespace Traffk.BackgroundJobServer
{
    class TenantManagementJobsRunner : BaseJobRunner, ITenantManagementJobs
    {
        private const string SqlAzureManagementApiResource = "https://management.azure.com/";
        private readonly ServiceClientCredentialFactory CredentialFactory;
        private readonly IOptions<Config> ConfigOptions;
        private readonly DbContextOptions<TraffkTenantModelDbContext> RdbOptions;
        private readonly TraffkTenantShardsDbContext Tdb;
        private readonly TraffkTenantShardManagerDbContext Smdb;
        private readonly IPasswordHasher<ApplicationUser> PasswordHasher;

        public class Config
        {
            public const string ConfigSectionName = "TenantManagementJobsRunnerConfig";
            public string ShardMapName { get; set; }
            public string SubscriptionId { get; set; }
            public string ResourceGroupName { get; set; }
            public string ServerName { get; set; }
            public string FullyQualifiedServerName { get; set; }
            public string TenantModelDatabaseName { get; set; }
            public string NewTenantDatabaseNameFormat { get; set; }
        }

        public TenantManagementJobsRunner(
            IJobInfoFinder jobInfoFinder,
            TraffkGlobalDbContext gdb,
            Serilog.ILogger logger,
            TraffkTenantShardsDbContext tdb,
            TraffkTenantShardManagerDbContext smdb,
            IPasswordHasher<ApplicationUser> passwordHasher,
            ServiceClientCredentialFactory credentialFactory, 
            IOptions<Config> configOptions,
            DbContextOptions<TraffkTenantModelDbContext> rdbOptions)
            : base(gdb, jobInfoFinder, logger)
        {
            CredentialFactory = credentialFactory;
            ConfigOptions = configOptions;
            RdbOptions = rdbOptions;
            Smdb = smdb;
            Tdb = tdb;
            PasswordHasher = passwordHasher;
        }

        void ITenantManagementJobs.CreateTenant(TenantCreationDetails details)
            => CreateTenantAsync(details).ExecuteSynchronously();

        private async Task CreateTenantAsync(TenantCreationDetails details)
        {
            var sqlAzureManagementConfiguration = ConfigOptions.Value;
            var cred = await CredentialFactory.GetAsync(SqlAzureManagementApiResource);
            var c = new SqlManagementClient(cred);
            c.SubscriptionId = sqlAzureManagementConfiguration.SubscriptionId;
            Stuff.Noop(c.Capabilities);
            foreach (var s in c.Servers.List())
            {
                if (s.Name == sqlAzureManagementConfiguration.ServerName)
                {
                    foreach (var db in c.Databases.ListByServer(sqlAzureManagementConfiguration.ResourceGroupName, s.Name))
                    {
                        if (db.Name == sqlAzureManagementConfiguration.TenantModelDatabaseName)
                        {
                            var newDatabaseName = string.Format(sqlAzureManagementConfiguration.NewTenantDatabaseNameFormat, details.TenantName);
                            await CopyDatabaseInitiateAsync(s, db, newDatabaseName);
                            var tenantId = await Tdb.TenantIdReserveAsync(newDatabaseName);
                            var id = new TenantInitializeDetails(details)
                            {
                                DatabaseServer = s.Name,
                                DatabaseName = newDatabaseName,
                                TenantId = tenantId
                            };
                            var initJob = BackgroundJob.ContinueWith<ITenantManagementJobs>(JobInfo.JobId.ToString(), j => j.InitializeNewTenantAsync(id));
                            initJob = BackgroundJob.ContinueWith<ITenantManagementJobs>(initJob, j => j.AddTenantToShardManagerAsync(id));
                            var bj = (IBackgroundJobClient)new TenantedBackgroundJobClient(GlobalContext, new HardcodedTraffkTenantFinder(tenantId));
                            bj.ContinueWith<ITenantJobs>(initJob, j => j.ReconfigureFiscalYears(new Bal.Settings.FiscalYearSettings { CalendarMonth = 1, CalendarYear = 2000, FiscalYear = 2000 }));
                            return;
                        }
                    }
                }
            }
        }

        /// <remarks>https://docs.microsoft.com/en-us/rest/api/</remarks>
        /// <remarks>https://docs.microsoft.com/en-us/rest/api/sql/databases</remarks>
        /// <remarks>https://docs.microsoft.com/en-us/azure/sql-database/sql-database-copy</remarks>
        /// <remarks>https://docs.microsoft.com/en-us/powershell/module/azure/start-azuresqldatabasecopy?view=azuresmps-4.0.0</remarks>
        /// <remarks>https://docs.microsoft.com/en-us/azure/azure-resource-manager/powershell-azure-resource-manager</remarks>
        /// <remarks>https://docs.microsoft.com/en-us/azure/sql-database/scripts/sql-database-copy-database-to-new-server-powershell?toc=%2fpowershell%2fmodule%2ftoc.json</remarks>
        private async Task CopyDatabaseInitiateAsync(
            Microsoft.Azure.Management.Sql.Models.Server sourceServer,
            Microsoft.Azure.Management.Sql.Models.Database sourceDatabase, 
            string destinationDatabase)
        {
            Requires.NonNull(sourceDatabase, nameof(sourceDatabase));
            Requires.Text(destinationDatabase, nameof(destinationDatabase));

            var req = new CopyDatabaseRequest
                {
                    location = sourceDatabase.Location,
                    properties = new CopyDatabaseRequest.Properties
                    {
                        sourceDatabaseId = sourceDatabase.Id
                    }
            };

            using (var c = new HttpClient())
            {
                using (var st = StreamHelpers.Create(req.ToJson()))
                {
                    var token = await CredentialFactory.GetTokenCredentialAsync(SqlAzureManagementApiResource);
                    c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    c.DefaultRequestHeaders.Add("CommandName", "New-AzureRmSqlDatabaseCopy");
                    c.DefaultRequestHeaders.Add("ParameterSetName", "__AllParameterSets");

                    var content = new StreamContent(st);
                    content.Headers.ContentType = MimeType.Application.Json;
                    var resp = await c.PutAsync(
                        $"https://management.azure.com/subscriptions/{ConfigOptions.Value.SubscriptionId}/resourceGroups/{ConfigOptions.Value.ResourceGroupName}/providers/Microsoft.Sql/servers/{sourceServer.Name}/databases/{destinationDatabase}?api-version=2014-04-01",
                        content);
                    var ret = resp.Content.ReadAsStringAsync();
                }
            }
        }

        private class MyDummyTenantFinder : ITraffkTenantFinder, IEnumerable<KeyValuePair<string, object>>
        {
            IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
            {
                yield return new KeyValuePair<string, object>(ConfigStringFormatter.CommonTerms.DatabaseName, DatabaseName);
            }

            IEnumerator IEnumerable.GetEnumerator()
                => CollectionHelpers.GetEnumerator(this);

            private readonly string DatabaseName;
            private readonly int TenantId;

            public MyDummyTenantFinder(string databaseName, int tenantId)
            {
                DatabaseName = databaseName;
                TenantId = tenantId;
            }

            Task<int> ITenantFinder<int>.GetTenantIdAsync()
                => Task.FromResult(TenantId);
        }

        async Task ITenantManagementJobs.AddTenantToShardManagerAsync(TenantInitializeDetails details)
        {
            var shardMapName = ConfigOptions.Value.ShardMapName;
            var shardMapId = (await Smdb.ShardMapsGlobal.Where(z => z.Name == shardMapName).FirstAsync()).ShardMapId;

            var shardId = Guid.NewGuid();
            var shardVersion = Guid.NewGuid();
            var lastOperationId = Guid.NewGuid();

            var finder = new MyDummyTenantFinder(details.DatabaseName, details.TenantId);
            using (var rdb = new TraffkTenantModelDbContext(RdbOptions, finder, new ConfigStringFormatter(finder) { }))
            {
                rdb.ShardMapsLocal.Add(new ShardMapsLocal {
                    ShardMapId = shardMapId,
                    Name = shardMapName,
                    MapType = 1,
                    KeyType = 1,
                    LastOperationId = lastOperationId
                });
                rdb.ShardsLocal.Add(new ShardsLocal {
                    ShardId = shardId,
                    Version = shardVersion,
                    ShardMapId = shardMapId,
                    Protocol = 0,
                    ServerName = details.DatabaseServer,
                    Port = 0,
                    DatabaseName = details.DatabaseName,
                    Status = 1,
                    LastOperationId = lastOperationId
                });
                await rdb.SaveChangesAsync();
            }

            Smdb.ShardsGlobal.Add(new ShardsGlobal {
                ShardId = shardId,
                Readable = true,
                Version = shardVersion,
                ShardMapId = shardMapId,
                Protocol = 0,
                ServerName = ConfigOptions.Value.FullyQualifiedServerName,
                Port = 0,
                DatabaseName = details.DatabaseName,
                Status = 1
            });
            await Smdb.SaveChangesAsync();
        }

        async Task ITenantManagementJobs.InitializeNewTenantAsync(TenantInitializeDetails details)
        {
            var finder = new MyDummyTenantFinder(details.DatabaseName, details.TenantId);
            using (var rdb = new TraffkTenantModelDbContext(RdbOptions, finder, new ConfigStringFormatter(finder) { }))
            {
                try
                {
                    rdb.Database.OpenConnection();
                }
                catch (Exception sex)
                {
                    throw new Exception("Database has not been created yet", sex);
                }
                using (var trans = await rdb.Database.BeginTransactionAsync())
                {
                    var t = await rdb.Tenants.FindAsync(details.TenantId);
                    if (t == null)
                    {
                        t = new Tenant
                        {
                            TenantId = details.TenantId,
                            TenantName = details.TenantName,
                            TenantType = Tenant.TenantTypes.Normal,                             
                        };
                        rdb.Tenants.Add(t);
                        rdb.Apps.Add(new App
                        {
                            Tenant = t,
                            AppType = AppTypes.Portal,
                            AppName = AppTypes.Portal.ToString(),
                        });
                        await rdb.SaveChangesAsync();
                        var r = new ApplicationRole
                        {
                            Tenant = t,
                            Name = "Bootstrap",
                            NormalizedName = "bootstrap",
                        };
                        rdb.Roles.Add(r);
                        var u = new ApplicationUser
                        {
                            Tenant = t,
                            Email = details.AdminUsername,
                            NormalizedEmail = details.AdminUsername.ToUpper(),
                            NormalizedUserName = details.AdminUsername.ToUpper(),
                        };
                        rdb.Users.Add(u);
                        await rdb.SaveChangesAsync();
                        u.PasswordHash = PasswordHasher.HashPassword(u, details.AdminPassword);
                        rdb.UserRoles.Add(new ApplicationUserRole
                        {
                            UserId = u.Id,
                            RoleId = r.Id,
                        });
                        foreach (var p in new[] {
                        PermissionNames.ManageJobs,
                        PermissionNames.ManageUsers,
                        PermissionNames.ManageRoles,
                        PermissionNames.ManageTenants,
                        PermissionNames.CustomerRelationshipData,
                        PermissionNames.DirectMessaging,
                        PermissionNames.ReleaseLog,
                    })
                        {
                            rdb.RoleClaims.Add(new RoleClaim
                            {
                                RoleId = r.Id,
                                ClaimType = PermissionHelpers.CreateClaimType(p),
                                ClaimValue = new PermissionClaimValue(true).ToJson()
                            });
                        }
                    }
                    await rdb.SaveChangesAsync();
                    trans.Commit();
                }
            }
        }

        public class CopyDatabaseRequest
        {
            public static CopyDatabaseRequest CreateFromJson(string json)
                => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<CopyDatabaseRequest>(json);

            public string ToJson() 
                => JsonConvert.SerializeObject(this);

            [JsonProperty("location")]
            public string location { get; set; }

            [JsonProperty("properties")]
            public Properties properties { get; set; }

            public class Properties
            {
                [JsonProperty("createMode")]
                public string createMode { get; set; } = "Copy";

                [JsonProperty("sourceDatabaseId")]
                public string sourceDatabaseId { get; set; }
            }
        }
    }
}
