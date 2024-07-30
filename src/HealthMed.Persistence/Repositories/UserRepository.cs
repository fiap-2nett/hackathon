using System.Threading.Tasks;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.ValueObjects;
using HealthMed.Persistence.Core.Primitives;

namespace HealthMed.Persistence.Repositories
{
    internal sealed class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        #region Constructors

        public UserRepository(IDbContext dbContext)
            : base(dbContext)
        { }

        #endregion

        #region IUserRepository Members

        public async Task<User> GetByEmailAsync(Email email)
            => await FirstOrDefaultAsync(user => user.Email.Value == email);

        public async Task<bool> IsEmailUniqueAsync(Email email)
            => !await AnyAsync(user => user.Email.Value == email);

        #endregion
    }
}
