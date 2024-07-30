using System.Threading.Tasks;
using HealthMed.Application.Contracts.Authentication;
using HealthMed.Domain.Enumerations;

namespace HealthMed.Application.Core.Abstractions.Services
{
    public interface IUserService
    {
        #region IUserService Members
        
        Task<TokenResponse> CreateAsync(string name, string cpf, string crm, string email, UserRoles userRole, string password);        

        #endregion
    }
}
