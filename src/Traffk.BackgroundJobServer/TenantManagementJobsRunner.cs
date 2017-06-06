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
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Rdb.TraffkGlobal;
using Traffk.Bal.Services;
using RevolutionaryStuff.Core.ApplicationParts;
using System.Collections;
using System.Linq;
using Traffk.Bal.Permissions;
using Microsoft.AspNetCore.Identity;
using Hangfire;
using Microsoft.Azure.SqlDatabase.ElasticScaleNetCore.ShardManagement;
using Microsoft.Extensions.Configuration;
using Traffk.Bal.Data.Rdb.TraffkTenantShards;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.BackgroundJobServer
{
    class TenantManagementJobsRunner : BaseJobRunner, ITenantManagementJobs
    {
        private const string SqlAzureManagementApiResource = "https://management.azure.com/";
        private readonly ServiceClientCredentialFactory CredentialFactory;
        private readonly IOptions<TenantManagementJobsRunnerConfiguration> ConfigurationOptions;
        private readonly DbContextOptions<TraffkTenantModelDbContext> RdbOptions;
        private readonly TraffkTenantShardsDbContext Tdb;
        private readonly IPasswordHasher<ApplicationUser> PasswordHasher;
        private readonly IConfiguration Config;

        public class TenantManagementJobsRunnerConfiguration
        {
            public string SubscriptionId { get; set; }
            public string ResourceGroupName { get; set; }
            public string ServerName { get; set; }
            public string TenantModelDatabaseName { get; set; }
            public string NewTenantDatabaseNameFormat { get; set; }
        }

        public TenantManagementJobsRunner(
            JobRunnerProgram jobRunnerProgram,
            TraffkGlobalDbContext gdb,
            Serilog.ILogger logger,
            IConfiguration config, 
            TraffkTenantShardsDbContext tdb,
            IPasswordHasher<ApplicationUser> passwordHasher,
            ServiceClientCredentialFactory credentialFactory, 
            IOptions<TenantManagementJobsRunnerConfiguration> configurationOptions,
            DbContextOptions<TraffkTenantModelDbContext> rdbOptions)
            : base(gdb, jobRunnerProgram, logger)
        {
            CredentialFactory = credentialFactory;
            ConfigurationOptions = configurationOptions;
            RdbOptions = rdbOptions;
            Tdb = tdb;
            PasswordHasher = passwordHasher;
            Config = config;
        }

        void ITenantManagementJobs.CreateTenant(TenantCreationDetails details)
            => CreateTenantAsync(details).ExecuteSynchronously();

        private async Task CreateTenantAsync(TenantCreationDetails details)
        {
            var sqlAzureManagementConfiguration = ConfigurationOptions.Value;
            var cred = await CredentialFactory.GetAsync(SqlAzureManagementApiResource);
            var c = new SqlManagementClient(cred);
            c.SubscriptionId = sqlAzureManagementConfiguration.SubscriptionId;
            Stuff.Noop(c.Capabilities);
            foreach (var s in c.Servers.List())
            {
                Stuff.Noop(s);
                foreach (var p in c.ElasticPools.ListByServer(sqlAzureManagementConfiguration.ResourceGroupName, s.Name))
                {
                    Stuff.Noop(p);
                }
                foreach (var db in c.Databases.ListByServer(sqlAzureManagementConfiguration.ResourceGroupName, s.Name))
                {
                    Stuff.Noop(db);
                }
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
                            var initJob = BackgroundJob.ContinueWith<ITenantManagementJobs>(JobId.ToString(), j => j.InitializeNewTenantAsync(id));
                            BackgroundJob.ContinueWith<ITenantManagementJobs>(initJob, j => j.AddTenantToShardManager(s.Name, newDatabaseName));
                            break;
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
                        $"https://management.azure.com/subscriptions/{ConfigurationOptions.Value.SubscriptionId}/resourceGroups/{ConfigurationOptions.Value.ResourceGroupName}/providers/Microsoft.Sql/servers/{sourceServer.Name}/databases/{destinationDatabase}?api-version=2014-04-01",
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

        private static object AddTenantToShardManagerAsyncLocker = new object();

        void ITenantManagementJobs.AddTenantToShardManager(string databaseServer, string databaseName)
        {
            lock (AddTenantToShardManagerAsyncLocker)
            {
                ShardMapManager shardMapManager;
                ShardMapManagerFactory.TryGetSqlShardMapManager(
                    Config.GetConnectionString(TraffkTenantShardsDbContext.DefaultDatabaseConnectionStringName),
                    ShardMapManagerLoadPolicy.Lazy,
                    out shardMapManager);
                ListShardMap<int> shardMap;
                shardMapManager.TryGetListShardMap(Config["ShardMapName"], out shardMap);
                var shardKey = shardMap.GetShards().Count() + 1;
                var shardLocation = new ShardLocation(databaseServer, databaseName);
                Shard shard;
                if (!shardMap.TryGetShard(shardLocation, out shard))
                {
                    shard = shardMap.CreateShard(shardLocation);
                }
                shardMap.CreatePointMapping(shardKey, shard);
            }
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
                        rdb.UserRoles.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>
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
                            rdb.RoleClaims.Add(new Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>
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
                var bj = (IBackgroundJobClient) new TenantedBackgroundJobClient(GlobalContext, finder);
                bj.ContinueWith<ITenantJobs>(JobId.ToString(), j => j.ReconfigureFiscalYears(new Bal.Settings.FiscalYearSettings { CalendarMonth = 1, CalendarYear=2000, FiscalYear=2000 }));
            }
        }

        public class CopyDatabaseRequest
        {
            public static CopyDatabaseRequest CreateFromJson(string json)
                => JsonConvert.DeserializeObject<CopyDatabaseRequest>(json);

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
