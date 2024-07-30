using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Repositories;
using HealthMed.Persistence.Core.Primitives;

namespace HealthMed.Persistence.Repositories
{
    internal sealed class ScheduleRepository : GenericRepository<Schedule, int>, IScheduleRepository
    {
        #region Constructors

        public ScheduleRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region IScheduleRepository Members

        #endregion
    }
}
