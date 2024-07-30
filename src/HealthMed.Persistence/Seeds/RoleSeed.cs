using System;
using System.Collections.Generic;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Persistence.Core.Primitives;

namespace HealthMed.Persistence.Seeds
{
    internal sealed class RoleSeed : EntitySeedConfiguration<Role, byte>
    {
        public override IEnumerable<object> Seed()
        {
            yield return new { Id = (byte)UserRoles.Doctor,     Name = "Médico"   };
            yield return new { Id = (byte)UserRoles.Patient,    Name = "Paciente" };
        }
    }
}
