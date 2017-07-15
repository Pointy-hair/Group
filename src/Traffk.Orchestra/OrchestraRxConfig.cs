namespace Traffk.Orchestra
{
    public class OrchestraRxConfig
    {
        public const string ConfigSectionName = "OrchestraRxOptions";

        public string BaseUrl { get; set; }
        public string AuthUrl { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
    }
}
