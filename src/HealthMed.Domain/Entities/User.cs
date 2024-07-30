using System;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Domain.Core.Primitives;
using HealthMed.Domain.Core.Utility;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Extensions;
using HealthMed.Domain.ValueObjects;

namespace HealthMed.Domain.Entities
{
    public class User : AggregateRoot<int>, IAuditableEntity
    {
        #region Private Fields

        private string _passwordHash;

        #endregion

        #region Properties

        public byte IdRole { get; private set; }

        public string Name { get; private set; }
        public string CPF { get; private set; }
        public string CRM { get; set; }
        public Email Email { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        #endregion

        #region Constructors

        private User()
        { }

        public User(string name, string cpf, string crm, Email email, UserRoles userRole, string passwordHash)
        {
            Ensure.NotEmpty(name, "The name is required.", nameof(name));
            Ensure.NotEmpty(cpf, "The CPF is required.", nameof(cpf));
            Ensure.NotEmpty(email, "The email is required.", nameof(email));
            Ensure.NotEmpty(passwordHash, "The password hash is required", nameof(passwordHash));

            Name = name;
            CPF = cpf;
            CRM = crm;
            Email = email;
            IdRole = (byte)userRole;
            _passwordHash = passwordHash;
        }

        #endregion

        #region Methods

        public bool VerifyPasswordHash(string password, IPasswordHashChecker passwordHashChecker)
            => !password.IsNullOrWhiteSpace() && passwordHashChecker.HashesMatch(_passwordHash, password);

        #endregion
    }
}
