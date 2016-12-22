using RevolutionaryStuff.Azure.DocumentDb;

namespace Traffk.Bal.Data.Ddb.Crm
{
    public class CommunicationOptingsDbSet : DdbDocSet<CrmDdbContext, CommunicationOpting>
    {
        public CommunicationOptingsDbSet(CrmDdbContext context)
            : base(context)
        { }
    }
}
