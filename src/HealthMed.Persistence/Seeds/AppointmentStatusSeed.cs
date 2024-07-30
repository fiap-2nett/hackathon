using System;
using System.Collections.Generic;
using HealthMed.Domain.Entities;
using HealthMed.Persistence.Core.Primitives;
using Enums = HealthMed.Domain.Enumerations;

namespace HealthMed.Persistence.Seeds
{
    internal sealed class AppointmentStatusSeed : EntitySeedConfiguration<AppointmentStatus, byte>
    {
        public override IEnumerable<object> Seed()
        {
            yield return new { Id = (byte)Enums.AppointmentStatus.Available,    Name = "Disponível" };
            yield return new { Id = (byte)Enums.AppointmentStatus.Busy,         Name = "Ocupado" };
            yield return new { Id = (byte)Enums.AppointmentStatus.Canceled,     Name = "Cancelado" };
        }
    }
}
