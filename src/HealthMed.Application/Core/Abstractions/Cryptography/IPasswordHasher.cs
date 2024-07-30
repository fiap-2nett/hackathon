using HealthMed.Domain.ValueObjects;

namespace HealthMed.Application.Core.Abstractions.Cryptography
{
    public interface IPasswordHasher
    {
        string HashPassword(Password password);
    }
}
