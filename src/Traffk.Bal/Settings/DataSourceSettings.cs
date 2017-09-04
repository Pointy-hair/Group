using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Traffk.Bal.Settings
{
    public class DataSourceSettings
    {
        [JsonProperty("decompressItems")]
        public bool DecompressItems { get; set; }

        [JsonIgnore]
        public bool IsFtp => this.FTP != null;

        [JsonIgnore]
        public bool IsWeb => this.Web != null;

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

            [JsonProperty("filePattern")]
            public string FilePattern { get; set; }
        }

        public class WebSettings
        {
            public class WebLoginPageConfig
            {
                [JsonProperty("loginPageUrl")]
                public Uri LoginPage { get; set; }

                [JsonProperty("usernameField")]
                public string UsernameFieldName { get; set; }

                [JsonProperty("passwordField")]
                public string PasswordFieldName { get; set; }
            }

            [JsonProperty("loginPageConfig")]
            public WebLoginPageConfig LoginPageConfig { get; set; }

            [JsonProperty("downloadUrl")]
            public IList<Uri> DownloadUrls { get; set; }

            [JsonProperty("credentialsKey")]
            public string CredentialsKeyUri { get; set; }
        }


        public static DataSourceSettings CreateFromJson(string json)
            => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<DataSourceSettings>(json);

        public string ToJson() 
            => JsonConvert.SerializeObject(this);

        [JsonProperty("ftp")]
        public FtpSettings FTP { get; set; }

        [JsonProperty("web")]
        public WebSettings Web { get; set; }
    }
}
