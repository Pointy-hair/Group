using Newtonsoft.Json;
using RevolutionaryStuff.Azure.DocumentDb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traffk.Bal.Data.Ddb.Crm
{
    [DocumentPreTrigger(CrmDdbContext.CommonTriggerNames.ApplyCreatedAtUtc)]
    [DocumentPostTrigger(CrmDdbContext.CommonTriggerNames.ValidateTenandIdExists)]
    [DocumentCollection(CrmDdbContext.DatabaseName, "CommunicationOptings")]
    public class CommunicationOpting : TenantedDdbEntity
    {
        public static readonly CommunicationOpting[] None = new CommunicationOpting[0];

        [EmailAddress]
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [Phone]
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("contactId")]
        public string ContactId { get; set; }

        [JsonProperty("createdAtUtc")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAtUtc { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt => CreatedAtUtc.ToLocalTime();

        [JsonProperty("allowedTopicsByMedium")]
        public Dictionary<string, string[]> AllowedTopicsByMedium { get; set; }

        [JsonProperty("disallowedTopicsByMedium")]
        public Dictionary<string, string[]> DisallowedTopicsByMedium { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
