using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Azure.DocumentDb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traffk.Bal.Data.Ddb.Crm
{
    [DocumentPreTrigger(CrmDdbContext.CommonTriggerNames.ApplyCreatedAtUtc)]
    [DocumentPostTrigger(CrmDdbContext.CommonTriggerNames.ValidateTenandIdExists)]
    [DocumentCollection(CrmDdbContext.DatabaseName, "CommunicationLogs")]
    public class CommunicationLog : TenantedDdbEntity
    {
        public static readonly CommunicationLog[] None = new CommunicationLog[0];

        [Required]
        [JsonProperty("recipientContactId")]
        public string RecipientContactId { get; set; }

        [EmailAddress]
        [JsonProperty("recipientEmailAddress")]
        public string RecipientEmailAddress { get; set; }

        [Phone]
        [JsonProperty("recipientPhoneNumber")]
        public string RecipientPhoneNumber { get; set; }

        [JsonProperty("createdAtUtc")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAtUtc { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt => CreatedAtUtc.ToLocalTime();

        [JsonProperty("communicationMedium")]
        public string CommunicationMedium { get; set; }

        [JsonProperty("communicationTopic")]
        public string CommunicationTopic { get; set; }

        [JsonProperty("communicationCampaign")]
        public string CommunicationCampaign { get; set; }

        [JsonProperty("jobId")]
        public int JobId { get; set; }

        [JsonProperty("directMessageSenderContactId")]
        public string DirectMessageSenderContactId { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("deliveryError")]
        public ExceptionError DeliveryError { get; set; }

        [JsonProperty("firstSeenAtUtc")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? FirstSeenAtUtc { get; set; }

        [JsonIgnore]
        public DateTime? FirstSeenAt => FirstSeenAtUtc?.ToLocalTime();

        [JsonIgnore]
        public int TrackedLinksTotalClicks
        {
            get
            {
                int cnt = 0;
                if (TrackedLinks != null)
                {
                    foreach (var tl in TrackedLinks)
                    {
                        if (tl.TrackedLinkType != TrackedLinkTypes.Anchor || tl.Visitors==null) continue;
                        cnt += tl.Visitors.Count;
                    }
                }
                return cnt;
            }
        } 

        public class TrackedLink
        {
            [Required]
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("mungedPathAndQuery")]
            public string MungedPathAndQuery { get; set; }

            [JsonProperty("redirectUrl")]
            public Uri RedirectUrl { get; set; }

            [JsonProperty("trackedLinkType")]
            public TrackedLinkTypes TrackedLinkType { get; set; }

            [JsonProperty("sequence")]
            public int Sequence { get; set; }

            public static string CreateId(TrackedLinkTypes type, int sequence) => $"{type}.{sequence}";

            public TrackedLink() { }

            public TrackedLink(string mungedPathAndQuery, Uri originalUrl, TrackedLinkTypes type, int sequence, string id=null)
            {
                MungedPathAndQuery = mungedPathAndQuery;
                RedirectUrl = originalUrl;
                TrackedLinkType = type;
                Sequence = sequence;
                Id = Stuff.CoalesceStrings(id, CreateId(TrackedLinkType, sequence));
            }

            [JsonProperty("visitors")]
            public List<WebActionLog> Visitors { get; set; }

            public void AddVisitor(WebActionLog visitor)
            {
                if (Visitors == null)
                {
                    Visitors = new List<WebActionLog>();
                }
                Visitors.Add(visitor);
            }
        }

        public enum TrackedLinkTypes
        {
            Anchor,
            Asset,
        }

        [JsonProperty("trackedLinks")]
        public IList<TrackedLink> TrackedLinks { get; set; }
    }
}
