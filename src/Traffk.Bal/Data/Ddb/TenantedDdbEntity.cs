using Newtonsoft.Json;
using RevolutionaryStuff.Azure.DocumentDb;

namespace Traffk.Bal.Data.Ddb
{
    public abstract class TenantedDdbEntity : DdbEntity, ITraffkTenanted, IDdbDataEntity
    {
        /// <summary>
        /// Foreign key to the tenant that owns this account
        /// </summary>
        [JsonProperty("tenantId")]
        [HideInPortal]
        //[PartitionKey]
        public int TenantId { get; set; }

        public override string ToString() => $"{base.ToString()} tenantId={TenantId}";
    }
}
