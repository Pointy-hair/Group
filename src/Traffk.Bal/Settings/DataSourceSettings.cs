using Newtonsoft.Json;
using System.Collections.Generic;

namespace Traffk.Bal.Settings
{
    public class DataSourceSettings
    {
        [JsonIgnore]
        public bool IsFtp => this.FTP != null;

        public class FtpSettings
        {
            [JsonProperty("hostname")]
            public string Hostname { get; set; }

            [JsonProperty("port")]
            public int Port { get; set; }

            [JsonProperty("folder")]
            public IList<string> FolderPaths { get; set; }

            [JsonProperty("credentialsKey")]
            public string CredentialsKeyUri { get; set; }
        }


        public static DataSourceSettings CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<DataSourceSettings>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);

        [JsonProperty("ftp")]
        public FtpSettings FTP { get; set; }
    }
}
