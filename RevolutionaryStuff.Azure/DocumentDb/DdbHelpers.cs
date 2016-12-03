using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Azure.DocumentDb.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevolutionaryStuff.Azure.DocumentDb
{
    public static class DdbHelpers
    {
        public static IList<DocSetEntry> EntityEntryList<T>(this DdbChangeTracker changeTracker, params EntityState[] states)
        {
            return changeTracker.Entries(states).Where(z => z.Entity.GetType().IsA(typeof(T))).ToList();
        }
        public static IList<T> EntityList<T>(this DdbChangeTracker changeTracker, params EntityState[] states)
        {
            return changeTracker.Entries(states).Where(z => z.Entity.GetType().IsA(typeof(T))).ConvertAll(z => (T)(object)z.Entity).ToList();
        }
        public static void PreSaveChanges<TTenantId>(this DdbContext ddb, TTenantId tenantId = default(TTenantId)) where
            TTenantId : IEquatable<TTenantId>
        {
            ddb.ChangeTracker.EntityList<IPreSave>(EntityState.Added, EntityState.Modified, EntityState.Unchanged).ForEach(z => z.PreSave());

            var noCreates = ddb.ChangeTracker.EntityList<IDontCreate>(EntityState.Added);
            if (noCreates.Count > 0)
            {
                var typeNames = noCreates.ConvertAll(z => z.GetType().Name).ToSet().ToCsv(false);
                throw new Exception(string.Format("Policy prevents following types from being created using EF: [{0}]", typeNames));
            }

            ddb.ChangeTracker.EntityList<IValidate>(EntityState.Added, EntityState.Modified).ForEach(z => z.Validate());

            if (!default(TTenantId).Equals(tenantId))
            {
                ddb.ChangeTracker.EntityList<ITenanted<TTenantId>>(EntityState.Added).ForEach(
                    tid =>
                    {
                        var existingTenantId = tid.TenantId;
                        if (default(TTenantId).Equals(existingTenantId))
                        {
                            tid.TenantId = tenantId;
                        }
                    }
                );
            }

            ddb.ChangeTracker.EntityEntryList<IDeleteOnSave>(EntityState.Modified, EntityState.Unchanged, EntityState.Added).ForEach(
                e =>
                {
                    if (!((IDeleteOnSave)e.Entity).IsMarkedForDeletion) return;
                    e.State = EntityState.Deleted;
                }
                );
        }
    }
}
