namespace Traffk.Bal.Data.Rdb
{
    public class AppHostItem : ITraffkTenanted
    {
        public int AppId { get; set; }
        public int TenantId { get; set; }
        public string ActualHostname { get; set; }
        public string PreferredHostname { get; set; }
        public string HostDatabaseName { get; set; }
    }
}
