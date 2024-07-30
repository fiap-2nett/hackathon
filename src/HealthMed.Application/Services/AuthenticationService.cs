using System;
using System.Threading.Tasks;
using HealthMed.Application.Contracts.Authentication;
using HealthMed.Application.Core.Abstractions.Authentication;
using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.ValueObjects;

namespace HealthMed.Application.Services
{
    internal sealed class AuthenticationService : IAuthenticationService
    {
        #region Read-Only Fields

        private readonly IJwtProvider _jwtProvider;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashChecker _passwordHashChecker;

        #endregion

        #region Constructors

        public AuthenticationService(IJwtProvider jwtProvider, IUserRepository userRepository, IPasswordHashChecker passwordHashChecker)
        {
            _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHashChecker = passwordHashChecker ?? throw new ArgumentNullException(nameof(passwordHashChecker));
        }

        #endregion

        #region IAuthenticationService Members

        public async Task<TokenResponse> Login(string email, string password)
        {
            var emailResult = Email.Create(email);

            var user = await _userRepository.GetByEmailAsync(emailResult);
            if (user is null)
                throw new DomainException(DomainErrors.Authentication.InvalidEmailOrPassword);

            var passwordValid = user.VerifyPasswordHash(password, _passwordHashChecker);
            if (!passwordValid)
                throw new DomainException(DomainErrors.Authentication.InvalidEmailOrPassword);

            return new TokenResponse(_jwtProvider.Create(user));
        }

        #endregion
    }
}
