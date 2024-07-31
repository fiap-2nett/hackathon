using System.Threading.Tasks;
using HealthMed.Application.Contracts.Authentication;
using HealthMed.Application.Contracts.Users;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;

namespace HealthMed.Application.Core.Abstractions.Services
{
    public interface IUserService
    {
        #region IUserService Members

        Task<DetailedUserResponse> GetUserByIdAsync(int idUser);
        Task<DetailedUserResponse> GetUserByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<int> CreateAsync(string name, string cpf, string crm, string email, UserRoles userRole, string password);
        
        #endregion
    }
}
