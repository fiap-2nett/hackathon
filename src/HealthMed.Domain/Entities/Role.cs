using System;
using HealthMed.Domain.Core.Primitives;
using HealthMed.Domain.Core.Utility;

namespace HealthMed.Domain.Entities
{
    public sealed class Role : Entity<byte>
    {
        #region Properties

        public string Name { get; private set; }                

        #endregion

        #region Constructors

        private Role()
        { }

        public Role(byte idRole, string name)
            : base(idRole)
        {
            Ensure.GreaterThan(idRole, 0, "The role identifier must be greater than zero.", nameof(idRole));
            Ensure.NotEmpty(name, "The role name is required.", nameof(name));

            Name = name;
        }

        #endregion
    }
}
