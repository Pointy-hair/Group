namespace TraffkPortal.Services.Sms
{
    public class TwilioSmsSenderOptions
    {
        public const string ConfigSectionName = "TwilioSmsSenderOptions";

        public string SID { get; set; }
        public string AuthToken { get; set; }
        public string SendNumber { get; set; }
        public string BaseUri { get; set; }
        public string RequestUri { get; set; }
    }
}
