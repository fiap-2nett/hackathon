using HealthMed.Domain.Core.Primitives;
using HealthMed.Domain.Core.Utility;

namespace HealthMed.Domain.Entities
{
    public sealed class AppointmentStatus : Entity<byte>
    {
        #region Properties

        public string Name { get; private set; }

        #endregion

        #region Constructors

        private AppointmentStatus()
        { }

        public AppointmentStatus(byte idAppointmentStatus, string name)
            : base(idAppointmentStatus)
        {
            Ensure.GreaterThan(idAppointmentStatus, 0, "The IdAppointmentStatus must be greater than zero.", nameof(idAppointmentStatus));
            Ensure.NotEmpty(name, "The name is required.", nameof(name));

            Name = name;
        }

        #endregion
    }
}
