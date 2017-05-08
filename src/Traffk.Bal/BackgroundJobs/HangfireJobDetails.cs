using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System.ComponentModel;

namespace Traffk.Bal.BackgroundJobs
{
    public class HangfireJobDetails
    {
        private string Method_p;

        [DisplayName("Job Description")]
        public string Method
        {
            get { return Method_p.ToTitleFriendlyString(); }
            set { Method_p = value; }
        }

        public static HangfireJobDetails CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<HangfireJobDetails>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
