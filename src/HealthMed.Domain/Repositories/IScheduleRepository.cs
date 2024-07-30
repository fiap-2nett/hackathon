using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Repositories
{
    public interface IScheduleRepository
    {
        #region IScheduleRepository Members

        void Insert(Schedule schedule);

        #endregion
    }
}
