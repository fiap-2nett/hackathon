namespace HealthMed.Application.Contracts.Appointment
{
    /// <summary>
    /// Represents the appointment status response.
    /// </summary>
    public sealed class AppointmentStatusResponse
    {
        /// <summary>
        /// Gets or sets the status identifier.
        /// </summary>
        public byte IdAppointmentStatus { get; set; }

        /// <summary>
        /// Gets or sets the status name.
        /// </summary>
        public string Name { get; set; }
    }
}
