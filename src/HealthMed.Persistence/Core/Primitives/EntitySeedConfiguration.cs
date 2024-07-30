using System;
using System.Collections.Generic;
using HealthMed.Persistence.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using HealthMed.Domain.Core.Primitives;

namespace HealthMed.Persistence.Core.Primitives
{
    internal abstract class EntitySeedConfiguration<TEntity, TKey> : IEntitySeedConfiguration
        where TEntity : Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        #region IEntitySeedConfiguration Members

        public abstract IEnumerable<object> Seed();

        public void Configure(ModelBuilder modelBuilder)
            => modelBuilder.Entity<TEntity>().HasData(Seed());

        #endregion
    }
}
