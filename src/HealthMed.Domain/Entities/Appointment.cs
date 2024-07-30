using System;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Domain.Core.Primitives;
using HealthMed.Domain.Core.Utility;

namespace HealthMed.Domain.Entities
{
    public class Appointment : AggregateRoot<int>, IAuditableEntity
    {
        #region Properties

        public int IdDoctor { get; private set; }
        public int IdPatient { get; private set; }
        public DateTime AppointmentDate { get; private set; }
        public bool HasBeenNotified { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        #endregion

        #region Constructors

        private Appointment()
        { }

        public Appointment(int idDoctor, int idPatient, DateTime appointmentDate, bool hasBeenNotified = false)
        {
            Ensure.GreaterThan(idDoctor, 0, "The IdDoctor must be greater than zero.", nameof(idDoctor));
            Ensure.GreaterThan(idPatient, 0, "The IdPatient must be greater than zero.", nameof(idPatient));
            Ensure.GreaterThan(appointmentDate, DateTime.MinValue, $"The AppointmentDate must be greater than {DateTime.MinValue:dd/MM/yyyy}.", nameof(appointmentDate));

            IdDoctor = idDoctor;
            IdPatient = idPatient;
            AppointmentDate = appointmentDate;
            HasBeenNotified = hasBeenNotified;
        }

        #endregion
    }
}
