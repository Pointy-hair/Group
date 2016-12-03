using RevolutionaryStuff.Core;
using RevolutionaryStuff.Azure.DocumentDb.Internal;
using System.Collections.Generic;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    public partial class DdbDocSet<TContext, TEntity> : IOrderedQueryable<TEntity> where TContext : DdbContext where TEntity : DdbEntity
    {
        public IAsyncEnumerable<TResult> Query<TResult>(JsonQuery query) where TResult : class
        {
            return Context.Query<TResult, TEntity>(this.DatabaseName, this.CollectionName, query);
        }

        public void Remove(TEntity entity)
        {
            var entry = Context.ChangeTracker.Find(entity);
            entry.State = EntityState.Deleted;
        }

        public DocSetEntry<TEntity> Add(TEntity entity)
        {
            return AddRange(entity)[0];
        }

        public IList<DocSetEntry<TEntity>> AddRange(params TEntity[] entities)
        {
            return AddRange((IEnumerable<TEntity>)entities);
        }

        public IList<DocSetEntry<TEntity>> AddRange(IEnumerable<TEntity> entities)
        {
            Requires.NonNull(entities, nameof(entities));
            return this.Context.ChangeTracker.Track(entities, EntityState.Added);
        }

        protected TContext Context { get; }

        protected readonly string DatabaseName;
        protected readonly string CollectionName;

        public DdbDocSet(TContext context)
        {
            Requires.NonNull(context, nameof(context));
            var dca = typeof(TEntity).GetCustomAttribute<DocumentCollectionAttribute>();
            Requires.NonNull(dca, typeof(TEntity).Name + ":" + nameof(DocumentCollectionAttribute));

            DatabaseName = dca.DatabaseName;
            CollectionName = dca.CollectionName;
            Context = context;
            Provider = new DdbDocSetQueryProvider(this);
            OnConstructed();
        }

        protected Task<TResult> CallSprocAsync<TResult>(string sprocName, params object[] args)
        {
            return Context.CallSprocAsync<TResult>(DatabaseName, CollectionName, sprocName, args);
        }

        protected virtual void OnConstructed()
        { }

        private async Task FetchAll()
        {
            if (!Fetched)
            {
                var docs = await Context.QueryDocuments<TEntity>(DatabaseName, CollectionName);
                Context.ChangeTracker.Track(docs.Documents, EntityState.Unchanged);
                Fetched = true;
            }
        }
        private bool Fetched;

        #region IEnumerable

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            FetchAll().ExecuteSynchronously();
            foreach (var e in Context.ChangeTracker.Entries<TEntity>(EntityState.Added, EntityState.Modified, EntityState.Unchanged))
            {
                yield return e.Entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            FetchAll().ExecuteSynchronously();
            foreach (var e in Context.ChangeTracker.Entries<TEntity>(EntityState.Added, EntityState.Modified, EntityState.Unchanged))
            {
                yield return e.Entity;
            }
        }

        #endregion

        #region IQueryable

        Type IQueryable.ElementType => typeof(TEntity);

        Expression IQueryable.Expression => Expression.Constant(this);

        public IQueryProvider Provider { get; private set; }

        #endregion
    }
}
