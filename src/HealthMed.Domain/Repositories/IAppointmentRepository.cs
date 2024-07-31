using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Repositories
{
    public interface IAppointmentRepository
    {
        #region IAppointmentRepository Members
        void Update(Appointment appointment);
        Task<Appointment> GetByAppointment(Appointment appointments);
        void Insert(Appointment appointment);
        void InsertRange(IReadOnlyCollection<Appointment> entities);
        Task<Appointment> GetByIdAsync(int idAppointment);
        Task<bool> IsOverlappingAsync(int idUserPatient, DateTime appointmentDate);

        #endregion
    }
}
