namespace Traffk.Bal.BackgroundJobs
{
    public class TenantCreationDetails
    {
        public string TenantName { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }

        public TenantCreationDetails() { }

        public TenantCreationDetails(TenantCreationDetails other)
        {
            if (other == null) return;
            this.TenantName = other.TenantName;
            this.AdminUsername = other.AdminUsername;
            this.AdminPassword = other.AdminPassword;
        }
    }

    public class TenantInitializeDetails : TenantCreationDetails
    {
        public string DatabaseServer { get; set; }
        public string DatabaseName { get; set; }
        public int TenantId { get; set; }

        public TenantInitializeDetails() { }

        public TenantInitializeDetails(TenantCreationDetails other)
            : base(other)
        { }

        public TenantInitializeDetails(TenantInitializeDetails other)
            : base(other)
        {
            this.DatabaseServer = other.DatabaseServer;
            this.DatabaseName = other.DatabaseName;
            this.TenantId = other.TenantId;
        }

}
}
