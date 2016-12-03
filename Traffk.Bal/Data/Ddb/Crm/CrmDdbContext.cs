using Microsoft.Extensions.Options;
using RevolutionaryStuff.Azure.DocumentDb;
using System.Threading.Tasks;
using RevolutionaryStuff.Core;

namespace Traffk.Bal.Data.Ddb.Crm
{
    public partial class CrmDdbContext : DdbContext
    {
        public const string DatabaseName = "CRM";

        public static class CommonTriggerNames
        {
            public const string ApplyCreatedAtUtc = "applyCreatedAtUtc";
            public const string ValidateTenandIdExists = "validateTenandIdExists";
        }

        public class CrmDdbOptions : DdbOptions
        { }

        public readonly ITraffkTenantFinder TenantFinder;

        public ContactsDbSet Contacts { get; set; }

        public CommunicationOptingsDbSet CommunicationOptings { get; set; }

        public CommunicationLogsDbSet CommunicationLogs { get; set; }

#if false
        public DdbDocSet<CrmDdbContext, MedicalClaim> MedicalClaims { get; set; }

        public DdbDocSet<CrmDdbContext, Pharmacy> Pharmacy { get; set; }

        public EligibilityDbSet Eligibility { get; set; }
#endif

        public CrmDdbContext(IOptions<CrmDdbOptions> options, ITraffkTenantFinder tenantFinder)
            : base(options)
        {
            TenantFinder = tenantFinder;
        }

        public override Task SaveChangesAsync()
        {
            var tenantId = TenantFinder.GetTenantIdAsync().ExecuteSynchronously();
            this.PreSaveChanges(tenantId);
            return base.SaveChangesAsync();
        }
    }
}
