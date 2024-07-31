using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Repositories
{
    public interface IAppointmentRepository
    {
        #region IAppointmentRepository Members

        void Insert(Appointment appointment);
        Task<Appointment> GetByIdAsync(int idAppointment);
        Task<bool> IsOverlappingAsync(int idUserPatient, DateTime appointmentDate);

        #endregion
    }
}
