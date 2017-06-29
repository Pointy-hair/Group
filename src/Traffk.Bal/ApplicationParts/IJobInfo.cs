namespace Traffk.Bal.ApplicationParts
{
    public interface IJobInfo
    {
        int? ContactId { get; }
        int JobId { get; }
        int? ParentJobId { get; }
        string RecurringJobId { get; }
        int? TenantId { get; }
    }
}