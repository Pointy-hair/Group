using RevolutionaryStuff.Azure.DocumentDb;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Traffk.Bal.Data.Ddb.Crm
{
    public abstract class CrmDbSet<TEntity> : DdbDocSet<CrmDdbContext, TEntity> where TEntity : DdbEntity
    {
        protected CrmDbSet(CrmDdbContext context)
            : base(context)
        { }

        public Task<GetCountsResult> GetCountsAsync(params Expression<Func<TEntity, object>>[] fieldNameExpressions) => Context.GetCountsAsync<TEntity>(fieldNameExpressions);

        public Task<GetCountsResult> GetContrainedFieldCountsAsync(bool includePhiFields) => Context.GetCountsAsync<TEntity>(true);
    }
}
