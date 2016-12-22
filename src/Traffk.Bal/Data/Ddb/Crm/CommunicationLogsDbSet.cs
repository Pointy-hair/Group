using RevolutionaryStuff.Azure.DocumentDb;

namespace Traffk.Bal.Data.Ddb.Crm
{
    public class CommunicationLogsDbSet : DdbDocSet<CrmDdbContext, CommunicationLog>
    {
        public CommunicationLogsDbSet(CrmDdbContext context)
            : base(context)
        { }
    }
}
