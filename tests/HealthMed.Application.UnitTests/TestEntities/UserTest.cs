using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.ValueObjects;

namespace HealthMed.Application.UnitTests.TestEntities
{
    internal class UserTest : User
    {
        public UserTest(int idUser, string name, string cpf, string crm, Email email, UserRoles userRole, string passwordHash)
            : base(name, cpf, crm, email, userRole, passwordHash)
        {
            Id = idUser;
        }
    }
}
