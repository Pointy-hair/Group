namespace Traffk.Bal.Settings
{
    public class ActiveDirectoryApplicationIdentificationConfig
    {
        public const string ConfigSectionName = "ActiveDirectoryApplicationIdentificationConfig";
        public string ApplicationId { get; set; }
        public string ApplicationSecret { get; set; }
    }
}
