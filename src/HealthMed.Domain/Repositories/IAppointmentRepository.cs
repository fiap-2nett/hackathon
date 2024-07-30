using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Repositories
{
    public interface IAppointmentRepository
    {
        #region IAppointmentRepository Members

        void Insert(Appointment appointment);

        #endregion
    }
}
