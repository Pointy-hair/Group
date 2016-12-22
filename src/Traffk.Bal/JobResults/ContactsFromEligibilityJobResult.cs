using Newtonsoft.Json;

namespace Traffk.Bal.JobResults
{
    public class ContactsFromEligibilityJobResult : JobResult
    {
        [JsonProperty("alreadySentRecipientContactIdCount")]
        public int AlreadySentRecipientContactIdCount { get; set; }

        [JsonProperty("matchingContactIdCount")]
        public int MatchingContactIdCount { get; set; }

        [JsonProperty("skippedDueToPreviousSendAttempt")]
        public int SkippedDueToPreviousSendAttempt { get; set; }

        public int IncrementSkippedDueToPreviousSendAttempt(int amt=1)
        {
            lock (this)
            {
                SkippedDueToPreviousSendAttempt += amt;
                return SkippedDueToPreviousSendAttempt;
            }
        }

        [JsonProperty("missingContactCount")]
        public int MissingContactCount { get; set; }

        public int IncrementMissingContactCount(int amt = 1)
        {
            lock (this)
            {
                MissingContactCount += amt;
                return MissingContactCount;
            }
        }

        [JsonProperty("exceptionsDuringMessageCreation")]
        public int ExceptionsDuringMessageCreation { get; set; }

        public int IncrementExceptionsDuringMessageCreation(int amt = 1)
        {
            lock (this)
            {
                ExceptionsDuringMessageCreation += amt;
                return ExceptionsDuringMessageCreation;
            }
        }
    }
}
