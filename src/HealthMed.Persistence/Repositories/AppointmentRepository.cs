using System.Threading.Tasks;
using System;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Repositories;
using HealthMed.Persistence.Core.Primitives;
using Enums = HealthMed.Domain.Enumerations;
using System.Collections.Generic;
using System.Runtime;

namespace HealthMed.Persistence.Repositories
{
    internal sealed class AppointmentRepository : GenericRepository<Appointment, int>, IAppointmentRepository
    {
        #region Constructors

        public AppointmentRepository(IDbContext dbContext)
            : base(dbContext)
        { }

        #endregion

        #region IAppointmentRepository Members

        public async Task<Appointment> GetByAppointment(Appointment appointment)
            => await FirstOrDefaultAsync(x =>
                x.IdDoctor == appointment.IdDoctor &&
                x.AppointmentDate == appointment.AppointmentDate &&
                x.IdAppointmentStatus != (byte)Enums.AppointmentStatus.Canceled);

        public async Task<bool> IsOverlappingAsync(int idUserPatient, DateTime appointmentDate)
            => await AnyAsync(x =>
                x.IdPatient == idUserPatient &&
                x.AppointmentDate == appointmentDate &&
                x.IdAppointmentStatus == (byte)Enums.AppointmentStatus.Busy);

        #endregion
    }
}