using System.Threading.Tasks;
using System;
using HealthMed.Domain.Entities;
using System.Collections.Generic;

namespace HealthMed.Domain.Repositories
{
    public interface IScheduleRepository
    {
        #region IScheduleRepository Members

        void Insert(Schedule schedule);
        void Update(Schedule schedule);
        Task<Schedule> GetByIdAsync(int scheduleId);
        void InsertRange(IReadOnlyCollection<Schedule> entities);
        Task<bool> HasScheduleConflictAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<bool> HasScheduleConflictAsync(int doctorId, int scheduleId, DateTime startDate, DateTime endDate);

        #endregion
    }
}
