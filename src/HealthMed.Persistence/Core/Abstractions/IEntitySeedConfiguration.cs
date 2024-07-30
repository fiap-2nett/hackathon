using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HealthMed.Persistence.Core.Abstractions
{
    internal interface IEntitySeedConfiguration
    {
        #region IEntitySeedConfiguration Members

        abstract IEnumerable<object> Seed();
        void Configure(ModelBuilder modelBuilder);

        #endregion
    }
}
