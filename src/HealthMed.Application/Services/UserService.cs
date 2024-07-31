using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HealthMed.Application.Contracts.Authentication;
using HealthMed.Application.Contracts.Users;
using HealthMed.Application.Core.Abstractions.Authentication;
using HealthMed.Application.Core.Abstractions.Cryptography;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static HealthMed.Domain.Errors.DomainErrors;

namespace HealthMed.Application.Services
{
    internal sealed class UserService : IUserService
    {
        #region Read-Only Fields

        private readonly IDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;


        #endregion

        #region Constructors

        public UserService(IDbContext dbContext,
            IUnitOfWork unitOfWork,
            IJwtProvider jwtProvider,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher
        )
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        #endregion

        #region IUserService Members

        public async Task<int> CreateAsync(string name, string cpf, string crm, string email, UserRoles userRole, string password)
        {
            var emailResult = Domain.ValueObjects.Email.Create(email);

            if (!await _userRepository.IsEmailUniqueAsync(emailResult))
                throw new DomainException(DomainErrors.User.DuplicateEmail);

            var passwordHash = _passwordHasher.HashPassword(Domain.ValueObjects.Password.Create(password));

            var user = new Domain.Entities.User(name, cpf, crm, emailResult, userRole, passwordHash);

            _userRepository.Insert(user);
            await _unitOfWork.SaveChangesAsync();

            return user.Id;
        }

        public async Task<DetailedUserResponse> GetUserByEmailAsync(string email)
        { 
            var userList = await _dbContext.Set<Domain.Entities.User, int>().AsNoTracking().ToListAsync();
            var user = userList.FirstOrDefault(u => u.Email == email);

            if (user == null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var role = await _dbContext.Set<Role, byte>().AsNoTracking()
                    .FirstOrDefaultAsync(r => Convert.ToByte(r.IdRole) == user.IdRole);

            var detailedUserResponse = new DetailedUserResponse
            {
                IdUser = user.Id,
                Name = user.Name,
                Email = user.Email,
                Cpf = user.CPF,
                Crm = user.CRM,
                Role = role != null ? new RoleResponse { IdRole = Convert.ToByte(role.IdRole), Name = role.Name } : null,
                CreatedAt = user.CreatedAt,
                LastUpdatedAt = user.LastUpdatedAt
            };

            return detailedUserResponse;
        }

        public async Task<DetailedUserResponse> GetUserByIdAsync(int idUser)
        {

           var user = await _dbContext.Set<Domain.Entities.User, int>()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Id == idUser);

            if (user is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var role = await _dbContext.Set<Role, byte>()
                                .AsNoTracking()
                                .FirstOrDefaultAsync(r => Convert.ToByte(r.IdRole) == user.IdRole);

            var detailedUserResponse = new DetailedUserResponse
            {
                IdUser = user.Id,
                Name = user.Name,
                Email = user.Email,
                Cpf = user.CPF,
                Crm = user.CRM,
                Role = role != null ? new RoleResponse { IdRole = Convert.ToByte(role.IdRole), Name = role.Name } : null,
                CreatedAt = user.CreatedAt,
                LastUpdatedAt = user.LastUpdatedAt
            };

            return detailedUserResponse;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            var userExists = await (
                 from user in _dbContext.Set<Domain.Entities.User, int>().AsNoTracking()
                 where user.Email == email
                 select user
             ).AnyAsync();

            return !userExists;
        }

        #endregion
    }
}
