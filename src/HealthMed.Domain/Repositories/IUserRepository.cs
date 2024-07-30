using System.Threading.Tasks;
using HealthMed.Domain.Entities;
using HealthMed.Domain.ValueObjects;

namespace HealthMed.Domain.Repositories
{
    public interface IUserRepository
    {
        #region IUserRepository Members

        Task<User> GetByIdAsync(int idUser);
        Task<User> GetByEmailAsync(Email email);
        Task<bool> IsEmailUniqueAsync(Email email);
        void Insert(User user);

        #endregion
    }
}
