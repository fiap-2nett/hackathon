using System;
using System.Threading.Tasks;
using HealthMed.Application.Contracts.Appointment;
using HealthMed.Application.Contracts.Common;

namespace HealthMed.Application.Core.Abstractions.Services
{
    public interface IAppointmentService
    {
        #region IAppointmentService Members

        Task<PagedList<AppointmentResponse>> ListAsync(ListAppointmentRequest listAppointmentRequest, int idUserDoctor, DateTime? fromDate);
        Task ReserveAsync(int idAppointment, int idUserDoctor, int idUserPatient);

        #endregion
    }
}
