using System;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Settings
{
    public class SmtpOptions : IValidate
    {
        [Required]
        [DisplayName("SMTP Host")]
        [JsonProperty("SmtpHost")]
        public string SmtpHost { get; set; }

        [Required]
        [DisplayName("SMTP Port")]
        [JsonProperty("SmtpPort")]
        public int SmtpPort { get; set; }

        [Required]
        [DisplayName("SMTP User")]
        [JsonProperty("SmtpUser")]
        public string SmtpUser { get; set; }

        [Required]
        [DisplayName("SMTP Password")]
        [JsonProperty("SmtpPassword")]
        [DataType(DataType.Password)]
        public string SmtpPassword { get; set; }

        [DisplayName("Local Domain")]
        [JsonProperty("LocalDomain")]
        public string LocalDomain { get; set; }

        public SmtpOptions() { }

        public SmtpOptions(SmtpOptions other)
        {
            if (other == null) return;
            SmtpHost = other.SmtpHost;
            SmtpPort = other.SmtpPort;
            SmtpUser = other.SmtpUser;
            SmtpPassword = other.SmtpPassword;
        }

        public void Validate()
        {
            Requires.Text(SmtpHost, nameof(SmtpHost));
            Requires.PortNumber(SmtpPort, nameof(SmtpPort));
            Requires.Text(SmtpUser, nameof(SmtpUser));
            Requires.Text(SmtpPassword, nameof(SmtpPassword));
        }

        public bool IsUsable()
        {
            return !String.IsNullOrEmpty(SmtpHost) && !String.IsNullOrEmpty(SmtpUser) &&
                    !String.IsNullOrEmpty(SmtpPassword);
        }
    }
}
