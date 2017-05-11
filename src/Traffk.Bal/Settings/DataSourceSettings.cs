using Newtonsoft.Json;

namespace Traffk.Bal.Settings
{
    public class DataSourceSettings
    {
        public class FtpSettings
        {
            public string Hostname { get; set; }
            public string Folderpath { get; set; }
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
