using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core;

namespace RevolutionaryStuff.Azure.DocumentDb.Internal
{
    public class DocSetEntry
    {
        public DdbEntity Entity { get; }

        public EntityState State { get; set; }

        public DocSetEntry(DdbEntity entity, EntityState state)
        {
            Requires.NonNull(entity, nameof(entity));

            Entity = entity;
            State = state;
        }
    }

    public class DocSetEntry<TEntity> : DocSetEntry where TEntity : DdbEntity
    {
        public new TEntity Entity => (TEntity)base.Entity;

        public DocSetEntry(TEntity entity, EntityState state)
            : base(entity, state)
        { }
    }
}
