using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Settings
{
    public class DeerwalkSettings
    {
        [Required]
        [JsonProperty("FtpHost")]
        public string FtpHost { get; set; }

        [Required]
        [JsonProperty("FtpPort")]
        public int FtpPort { get; set; }

        [Required]
        [JsonProperty("FtpUser")]
        public string FtpUser { get; set; }

        [Required]
        [JsonProperty("FtpPassword")]
        [DataType(DataType.Password)]
        public string FtpPassword { get; set; }

        [Required]
        [JsonProperty("FtpFolder")]
        public string FtpFolder { get; set; }

        public DeerwalkSettings() { }

        public DeerwalkSettings(DeerwalkSettings settings)
        {
            if (settings != null)
            {
                FtpHost = settings.FtpHost;
                FtpPort = settings.FtpPort;
                FtpUser = settings.FtpUser;
                FtpPassword = settings.FtpPassword;
                FtpFolder = settings.FtpFolder;
            }
        }
    }
}
