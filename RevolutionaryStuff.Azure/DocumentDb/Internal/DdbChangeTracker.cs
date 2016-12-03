using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevolutionaryStuff.Azure.DocumentDb.Internal
{
    public class DdbChangeTracker
    {
        public readonly IDictionary<Type, IDictionary<string, DocSetEntry>> EntityByIdByType = new Dictionary<Type, IDictionary<string, DocSetEntry>>();

        public DocSetEntry<TEntity> Find<TEntity>(TEntity existing) where TEntity : DdbEntity
        {
            var d = EntityByIdByType.FindOrDefault(typeof(TEntity));
            return d == null ? null : (DocSetEntry<TEntity>) d.FindOrDefault(existing.InstanceKey);
        }

        public IEnumerable<DocSetEntry> Entries(params EntityState[] states)
        {
            bool anyState = states == null || states.Length == 0;
            foreach (var d in EntityByIdByType.Values)
            {
                foreach (var e in d.Values)
                {
                    if (anyState || states.Contains(e.State))
                    {
                        yield return e;
                    }
                }
            }
        }

        public IEnumerable<DocSetEntry<TEntity>> Entries<TEntity>(params EntityState[] states) where TEntity : DdbEntity
        {
            bool anyState = states == null || states.Length == 0;
            var d = EntityByIdByType[typeof(TEntity)];
            if (d != null)
            {
                foreach (var e in d.Values)
                {
                    if (anyState || states.Contains(e.State))
                    {
                        yield return (DocSetEntry<TEntity>)e;
                    }
                }
            }
        }

        internal IList<DocSetEntry<TEntity>> Track<TEntity>(IEnumerable<TEntity> entities, EntityState stateOfUnfoundEntity) where TEntity : DdbEntity
        {
            var ret = new List<DocSetEntry<TEntity>>();
            lock (this)
            {
                var d = EntityByIdByType.FindOrCreate(typeof(TEntity), _ => new Dictionary<string, DocSetEntry>());
                foreach (var e in entities)
                {
                    var key = e.InstanceKey;
                    DocSetEntry val;
                    if (!d.TryGetValue(key, out val))
                    {
                        val = new DocSetEntry<TEntity>(e, stateOfUnfoundEntity);
                        d[key] = val;
                    }
                    ret.Add((DocSetEntry<TEntity>)val);
                }
            }
            return ret;
        }
    }
}
