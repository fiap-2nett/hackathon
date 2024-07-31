using System;
using System.Collections.Generic;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Domain.Core.Primitives;
using HealthMed.Domain.Core.Utility;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using Enums = HealthMed.Domain.Enumerations;

namespace HealthMed.Domain.Entities
{
    public class Appointment : AggregateRoot<int>, IAuditableEntity
    {
        #region Properties

        public int IdDoctor { get; private set; }
        public int? IdPatient { get; private set; }

        public DateTime AppointmentDate { get; private set; }
        public bool HasBeenNotified { get; private set; }

        public byte IdAppointmentStatus { get; private set; }
        public DateTime? CanceledAt { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        #endregion

        #region Constructors

        private Appointment()
        { }

        public Appointment(int idDoctor, DateTime appointmentDate)
        {
            Ensure.GreaterThan(idDoctor, 0, "The IdDoctor must be greater than zero.", nameof(idDoctor));
            Ensure.GreaterThan(appointmentDate, DateTime.MinValue, $"The AppointmentDate must be greater than {DateTime.MinValue:dd/MM/yyyy}.", nameof(appointmentDate));

            IdDoctor = idDoctor;
            AppointmentDate = appointmentDate;
            IdAppointmentStatus = (byte)Enums.AppointmentStatus.Available;
        }

        #endregion

        #region Public Methods

        public void CancelAppointment()
        {
            IdAppointmentStatus = (byte)Enums.AppointmentStatus.Canceled;
            CanceledAt = DateTime.UtcNow;
        }

        public void Reserve(User userPatient)
        {
            if (userPatient is null)
                throw new DomainException(DomainErrors.User.NotFound);

            if (IdAppointmentStatus != (byte)Enums.AppointmentStatus.Available)
                throw new InvalidPermissionException(DomainErrors.Appointment.CannotBeReserved);

            IdPatient = userPatient.Id;
            HasBeenNotified = true;
            IdAppointmentStatus = (byte)Enums.AppointmentStatus.Busy;
        }

        #endregion

        #region Private Methods

        public static IList<Appointment> BuildAppointmentListFromSchedules(int idDoctor, DateTime startDate, DateTime endDate)
        {
            List<Appointment> appointmentsList = new List<Appointment>();
            var currentStart = startDate;
            while (currentStart < endDate)
            {
                appointmentsList.Add(new Appointment(idDoctor, currentStart));
                currentStart = currentStart.AddHours(1);
            }

            return appointmentsList;
        }

        public static IReadOnlyCollection<Appointment> BuildAppointmentListFromSchedules(int idDoctor, IReadOnlyCollection<Schedule> schedules)
        {
            List<Appointment> appointmentsList = new List<Appointment>();

            foreach (var schedule in schedules)
            {
                var currentStart = schedule.StartDate;
                while (currentStart < schedule.EndDate)
                {
                    appointmentsList.Add(new Appointment(idDoctor, currentStart));
                    currentStart = currentStart.AddHours(1);
                }
            }

            return appointmentsList;
        }

        #endregion
    }
}
