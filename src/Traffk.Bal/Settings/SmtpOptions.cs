using RevolutionaryStuff.Core.ApplicationParts;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using RevolutionaryStuff.Core;

namespace Traffk.Bal.Settings
{
    public class SmtpOptions : IValidate
    {
        [Required]
        [DataMember(Name = "SmtpHost")]
        public string SmtpHost { get; set; }

        [Required]
        [DataMember(Name = "SmtpPort")]
        public int SmtpPort { get; set; }

        [Required]
        [DataMember(Name = "SmtpUser")]
        public string SmtpUser { get; set; }

        [Required]
        [DataMember(Name = "SmtpPassword")]
        [DataType(DataType.Password)]
        public string SmtpPassword { get; set; }

        [DataMember(Name = "LocalDomain")]
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
    }
}
