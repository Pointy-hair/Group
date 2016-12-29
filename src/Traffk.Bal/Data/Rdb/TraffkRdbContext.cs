using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Traffk.Bal.Services;

namespace Traffk.Bal.Data.Rdb
{
    public partial class TraffkRdbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        protected readonly ITraffkTenantFinder TenantFinder;

        public override int SaveChanges(bool acceptAllChangesOnSuccess) => SaveChangesAsync().ExecuteSynchronously();

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.PreSaveChanges(()=>TenantFinder.GetTenantIdAsync().ExecuteSynchronously());
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sso = optionsBuilder.Options.FindExtension<SqlServerOptionsExtension>();
            if (sso != null && Configger!=null)
            {
                var connectionString = Configger.Transform(sso.ConnectionString);
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                base.OnConfiguring(optionsBuilder);
            }
        }

        private readonly ConfigStringFormatter Configger;

        public TraffkRdbContext(DbContextOptions<TraffkRdbContext> options, ITraffkTenantFinder tenantFinder, ConfigStringFormatter configger)
            : base(options)
        {
            TenantFinder = tenantFinder;
            Configger = configger;
        }

        public async Task<SystemCommunication> GetSystemCommunication(string communicationPurpose, string communicationMedium, int? applicationId=null, int? tenantId=null)
        {
            SystemCommunication.CommunicationPurposes.RequiresPredefinedOrGeneralPurpose(communicationPurpose, nameof(communicationPurpose));
            SystemCommunication.CommunicationMediums.RequiresPredefined(communicationMedium, nameof(communicationMedium));

            if (tenantId == null)
            {
                tenantId = await TenantFinder.GetTenantIdAsync();
            }

            ///BUGBUG: must order by so that we get the communication for this application over the null application
            return await SystemCommunications.Include(c=>c.MessageTemplate).FirstOrDefaultAsync(
                c =>
                c.CommunicationMedium == communicationMedium &&
                c.CommunicationPurpose == communicationPurpose &&
                (c.ApplicationId == applicationId || c.ApplicationId==null) &&
                c.TenantId == tenantId);
        }

        public async Task<SystemCommunication> GetSystemCommunication(int communicationId, int? tenantId = null)
        {
            if (tenantId == null)
            {
                tenantId = await TenantFinder.GetTenantIdAsync();
            }

            return await SystemCommunications.Include(c => c.MessageTemplate).FirstOrDefaultAsync(
                c =>
                c.SystemCommunicationId == communicationId &&
                c.TenantId == tenantId);
        }

        public async Task<bool> AddNextScheduledBlasts(int? specificBlastId=null, bool allTenants=false, bool deleteUpcomingNonConformantBlasts=true)
        {
            var parentBlasts = ZCommunicationBlasts.AsQueryable();
            if (specificBlastId != null)
            {
                parentBlasts = parentBlasts.Where(b => b.CommunicationBlastId == specificBlastId.Value);
            }
            else if (!allTenants)
            {
                var tenantId = await TenantFinder.GetTenantIdAsync();
                parentBlasts = parentBlasts.Where(b => b.TenantId == tenantId);
            }
            parentBlasts = parentBlasts.Where(b => b.Job != null);
            bool saveRequired = false;
            foreach (var b in parentBlasts)
            {
                var nextOccurrence = b.CommunicationBlastSettings?.Recurrence?.NextOccurrence;
                if (nextOccurrence == null) continue;
                var kids = await ZCommunicationBlasts.Where(z => z.ParentCommunicationBlastId == b.CommunicationBlastId && z.Job != null).Include(z => z.Job).ToListAsync();
                var nextScheduled = kids.Where(z=>z.Job.JobStatus!=JobStatuses.Cancelled).OrderByDescending(z => z.Job.DontRunBeforeUtc).FirstOrDefault();
                if (nextScheduled == null)
                {
                    var newBlast = new ZCommunicationBlast()
                    {
                        CommunicationBlastTitle = b.CommunicationBlastTitle,
                        CommunicationMedium = b.CommunicationMedium,
                        TopicName = b.TopicName,
                        CampaignName = b.CampaignName,
                        MessageTemplateId = b.MessageTemplateId,
                        ParentCommunicationBlast = b,
                        TenantId = b.TenantId,
                        Job = new Job
                        {
                            JobStatus = JobStatuses.Queued,
                            DontRunBeforeUtc = nextOccurrence.StartAtUtc,
                            TenantId = b.TenantId,
                            JobType = JobTypes.CommunicationBlast,
                        }
                    };
                    if (deleteUpcomingNonConformantBlasts)
                    {
                        var toRemove = kids.Where(z => z.Job.JobStatus == JobStatuses.Queued).ToList();
                        if (toRemove.Count > 0)
                        {
                            ZCommunicationBlasts.RemoveRange(toRemove);
                            Jobs.RemoveRange(toRemove.ConvertAll(z => z.Job));
                        }
                    }
                    ZCommunicationBlasts.Add(newBlast);
                }
            }
            return saveRequired;
        }

        public async Task<GetCountsResult> GetFieldCountsAsync<TDataEntity>(params Expression<Func<TDataEntity, object>>[] fieldNameExpressions) where TDataEntity : IRdbDataEntity
        {
            var fieldNames = new string[fieldNameExpressions.Length];
            for (int z = 0; z < fieldNames.Length; ++z)
            {
                var e = fieldNameExpressions[z];
                var mis = e.GetMembers();
                string name = mis.ConvertAll(mi => mi.Name).Join(".");
                fieldNames[z] = name;
            }
            return await GetFieldCountsAsync<TDataEntity>(fieldNames);
        }

        public async Task<GetCountsResult> GetFieldCountsAsync<TDataEntity>(params string[] fieldNames) where TDataEntity : IRdbDataEntity
        {
            var t = typeof(TDataEntity);
            var ta = t.GetCustomAttribute<TableAttribute>();
            var tenantId = await TenantFinder.GetTenantIdAsync();
            var items = await GetFieldCountsAsync(ta.Schema, ta.Name, tenantId, fieldNames.ToCsv(false));
            return new GetCountsResult(items);
        }

        public Task<GetCountsResult> GetContrainedFieldCountsAsync<TDataEntity>(bool includePhiFields) where TDataEntity : IRdbDataEntity
        {
            var t = typeof(TDataEntity);
            var fieldNames = new List<string>();
            foreach (var pi in t.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                if (pi.GetCustomAttribute<NotMappedAttribute>() != null) continue;
                var phi = pi.GetCustomAttribute<ProtectedHealthInformationIdentifierAttribute>();
                if (!ConstrainedDataAttribute.IsContrained(pi)) continue;
                if (phi != null || !includePhiFields) continue;
                var fieldName = pi.GetCustomAttribute<ColumnAttribute>()?.Name ?? pi.Name;
                fieldNames.Add(fieldName);
            }
            return GetFieldCountsAsync<TDataEntity>(fieldNames.ToArray());
        }
    }
}
