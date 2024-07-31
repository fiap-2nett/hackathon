using System;
using System.Threading.Tasks;
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

        public async Task<bool> HasScheduleConflictAsync(int doctorId, DateTime startDate,  DateTime endDate)
        {
            return await AnyAsync(s => s.IdDoctor == doctorId &&
                                 ((s.StartDate < endDate && s.EndDate > startDate) ||
                                 (s.StartDate < endDate && s.EndDate > startDate)));
        }

        public async Task<bool> HasScheduleConflictAsync(int doctorId, int scheduleId, DateTime startDate, DateTime endDate)
        {
            return await AnyAsync(s => s.IdDoctor == doctorId &&
                                     s.Id != scheduleId && 
                                     ((s.StartDate < endDate && s.EndDate > startDate) ||
                                      (s.StartDate < endDate && s.EndDate > startDate)));

        }

        #endregion
    }
}
