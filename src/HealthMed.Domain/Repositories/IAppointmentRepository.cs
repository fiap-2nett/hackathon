using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthMed.Domain.Entities;

namespace HealthMed.Domain.Repositories
{
    public interface IAppointmentRepository
    {
        #region IAppointmentRepository Members

        Task<Appointment> GetByIdAsync(int idAppointment);        
        Task<Appointment> GetByDoctorAndDateAsync(int idUserDoctor, DateTime appointmentDate);
        Task<bool> IsOverlappingAsync(int idUserPatient, DateTime appointmentDate);

        void Update(Appointment appointment);
        void Insert(Appointment appointment);
        void InsertRange(IReadOnlyCollection<Appointment> entities);

        #endregion
    }
}
