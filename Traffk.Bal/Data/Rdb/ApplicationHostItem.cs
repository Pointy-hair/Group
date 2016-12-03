namespace Traffk.Bal.Data.Rdb
{
    public class ApplicationHostItem : ITraffkTenanted
    {
        public int ApplicationId { get; set; }
        public int TenantId { get; set; }
        public string ActualHostname { get; set; }
        public string PreferredHostname { get; set; }
    }
}
