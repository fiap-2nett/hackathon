using System;

namespace HealthMed.Application.Contracts.Appointment
{
    /// <summary>
    /// Represents the appointment response.
    /// </summary>
    public sealed class AppointmentResponse
    {
        /// <summary>
        /// Gets or sets the appointment identifier.
        /// </summary>
        public int IdAppointment { get; set; }

        /// <summary>
        /// Gets or sets the appointment date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the appointment status.
        /// </summary>
        public AppointmentStatusResponse Status { get; set; }
    }
}
