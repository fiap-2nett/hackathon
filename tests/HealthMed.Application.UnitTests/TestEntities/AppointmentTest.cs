using System;
using HealthMed.Domain.Entities;

namespace HealthMed.Application.UnitTests.TestEntities
{
    internal class AppointmentTest : Appointment
    {
        public AppointmentTest(int idAppointment, User userDoctor, DateTime appointmentDate)
            : base(userDoctor.Id, appointmentDate)
        {
            Id = idAppointment;
        }

        public static AppointmentTest Create(int idAppointment, User userDoctor, DateTime appointmentDate, User userPatient = null, bool toCanceled = false)
        {
            var appointment = new AppointmentTest(idAppointment, userDoctor, appointmentDate);

            if (userPatient is not null)
                appointment.Reserve(userPatient);

            if (toCanceled)
                appointment.Cancel();

            return appointment;
        }
    }
}
