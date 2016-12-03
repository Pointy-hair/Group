namespace Traffk.Bal.Data.Ddb.Crm
{
    public class ContactsDbSet : CrmDbSet<Zontact>
    {
        public ContactsDbSet(CrmDdbContext context)
            : base(context)
        { }
    }
}
