using System;
using System.Threading.Tasks;
using HealthMed.Application.Contracts.Authentication;
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

        public async Task<TokenResponse> CreateAsync(string name, string cpf, string crm, string email, UserRoles userRole, string password)
        {
            var emailResult = Email.Create(email);

            if (!await _userRepository.IsEmailUniqueAsync(emailResult))
                throw new DomainException(DomainErrors.User.DuplicateEmail);

            var passwordHash = _passwordHasher.HashPassword(Password.Create(password));
            var user = new User(name, cpf, crm, emailResult, userRole, passwordHash);

            _userRepository.Insert(user);
            await _unitOfWork.SaveChangesAsync();

            return new TokenResponse(_jwtProvider.Create(user));
        }

        #endregion
    }
}
