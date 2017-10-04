using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.Caching;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Traffk.Bal.Communications;
using Traffk.Bal.Services;
using Traffk.Bal.Settings;

namespace Traffk.Bal.Data.Rdb.TraffkTenantModel
{
    public partial class TraffkTenantModelDbContext : 
        IdentityDbContext<ApplicationUser, ApplicationRole, string, UserClaim, ApplicationUserRole, UserLogin, RoleClaim, UserToken>,
        ICreativeSettingsFinder
    {
        public const string DefaultDatabaseConnectionStringName = "TraffkTenantPortal";

        protected readonly ITraffkTenantFinder TenantFinder;

        public override int SaveChanges(bool acceptAllChangesOnSuccess) 
            => SaveChangesAsync().ExecuteSynchronously();

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
                ConnectionString = Configger.Transform(sso.ConnectionString);
                optionsBuilder.UseSqlServer(ConnectionString);
            }
            else
            {
                base.OnConfiguring(optionsBuilder);
            }
        }

        private string ConnectionString;

        private int GetTableObjectId(IRdbDataEntity e)
            => GetDatabaseObjectId(AttributeStuff.GetCustomAttribute<TableAttribute>(e.GetType()));

        private int GetDatabaseObjectId(TableAttribute ta)
            => Cache.DataCacher.FindOrCreate(
                Cache.CreateKey(ConnectionString, ta.Schema.ToLower(), ta.Name.ToLower()),
                () => SchemaTables.ConvertAll(z => Tuple.Create(
                        Cache.CreateKey(ConnectionString, z.SchemaName.ToLower(), z.TableName.ToLower()),
                        new CacheEntry<int>(z.ObjectId)
                        ))
                ).Value;

        public void AttachNote(Contact creator, string subject, string body, int? parentNoteId = null, params IRdbDataEntity[] attachmentSites)
        {
            Requires.NonNull(creator, nameof(creator));
            Requires.Between(attachmentSites.Length, nameof(attachmentSites), 1);

            var n = new Note
            {
                CreatedByContact = creator,
                Body = body,
                Subject = subject,
                ParentNoteId = parentNoteId
            };
            Notes.Add(n);
            foreach (var site in attachmentSites)
            {
                try
                {
                    int pk = Convert.ToInt32(((IPrimaryKey)site).Key);
                    NoteTargets.Add(new NoteTarget
                    {
                        Note = n,
                        TableObjectId = GetTableObjectId(site),
                        TablePkIntVal = pk
                    });
                }
                catch (Exception)
                { }
            }
        }

        public IQueryable<Note> GetAttachedNotes(IRdbDataEntity attachmentSite)
        {
            try
            {
                int pk = Convert.ToInt32(((IPrimaryKey)attachmentSite).Key);
                var tid = GetTableObjectId(attachmentSite);

                return
                    (
                    from nt in this.NoteTargets
                    where nt.TableObjectId == tid && nt.TablePkIntVal == pk
                    select nt.Note
                    ).Include(n => n.CreatedByContact);
            }
            catch (Exception)
            {
                return Note.None.ToList().AsQueryable();
            }
        }

        private readonly ConfigStringFormatter Configger;

        public TraffkTenantModelDbContext(DbContextOptions<TraffkTenantModelDbContext> options, ITraffkTenantFinder tenantFinder, ConfigStringFormatter configger)
            : base(options)
        {
            TenantFinder = tenantFinder;
            Configger = configger;
        }

        public Task<bool> AddNextScheduledBlasts(int? specificBlastId=null, bool allTenants=false, bool deleteUpcomingNonConformantBlasts=true)
        {
            throw new NotImplementedException();
            /*

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
            */
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
                var phi = pi.GetCustomAttribute<ProtectedHealthInformationAttribute>();
                if (!ConstrainedDataAttribute.IsContrained(pi)) continue;
                if (phi != null || !includePhiFields) continue;
                var fieldName = pi.GetCustomAttribute<ColumnAttribute>()?.Name ?? pi.Name;
                fieldNames.Add(fieldName);
            }
            return GetFieldCountsAsync<TDataEntity>(fieldNames.ToArray());
        }

        CreativeSettings ICreativeSettingsFinder.FindSettingsByName(string name)
            => this.Creatives.FirstOrDefault(z => z.CreativeTitle == name)?.CreativeSettings;

        CreativeSettings ICreativeSettingsFinder.FindSettingsById(int id)
            => this.Creatives.Find(id)?.CreativeSettings;
    }
}
