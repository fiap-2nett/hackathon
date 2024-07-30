using HealthMed.Domain.Entities;

namespace HealthMed.Application.Core.Abstractions.Authentication
{
    public interface IJwtProvider
    {
        #region IJwtProvider Members

        string Create(User user);

        #endregion
    }
}
