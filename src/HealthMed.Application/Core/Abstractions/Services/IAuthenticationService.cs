using System.Threading.Tasks;
using HealthMed.Application.Contracts.Authentication;

namespace HealthMed.Application.Core.Abstractions.Services
{
    public interface IAuthenticationService
    {
        #region IAuthenticationService Members

        Task<TokenResponse> Login(string email, string password);

        #endregion
    }
}
