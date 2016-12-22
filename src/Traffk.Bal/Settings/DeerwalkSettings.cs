using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Traffk.Bal.Settings
{
    [DataContract]
    public class DeerwalkSettings
    {
        [Required]
        [DataMember(Name = "FtpHost")]
        public string FtpHost { get; set; }

        [Required]
        [DataMember(Name = "FtpPort")]
        public int FtpPort { get; set; }

        [Required]
        [DataMember(Name = "FtpUser")]
        public string FtpUser { get; set; }

        [Required]
        [DataMember(Name = "FtpPassword")]
        [DataType(DataType.Password)]
        public string FtpPassword { get; set; }

        [Required]
        [DataMember(Name = "FtpFolder")]
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
