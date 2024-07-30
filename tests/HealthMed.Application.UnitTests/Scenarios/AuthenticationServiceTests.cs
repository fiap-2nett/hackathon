using Moq;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using HealthMed.Domain.Errors;
using Microsoft.Extensions.Options;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.ValueObjects;
using HealthMed.Domain.Repositories;
using HealthMed.Application.Services;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Infrastructure.Cryptography;
using HealthMed.Infrastructure.Authentication;
using HealthMed.Application.UnitTests.TestEntities;
using HealthMed.Infrastructure.Authentication.Settings;
using HealthMed.Application.Core.Abstractions.Authentication;

namespace HealthMed.Application.UnitTests.Scenarios
{
    public sealed class AuthenticationServiceTests
    {
        #region Read-Only Fields

        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHashChecker _passwordHashChecker;

        private readonly Mock<IUserRepository> _userRepositoryMock;

        #endregion

        #region Constructors

        public AuthenticationServiceTests()
        {
            _userRepositoryMock = new();

            _jwtProvider = new JwtProvider(JwtOptions);
            _passwordHashChecker = new PasswordHasher();
        }

        #endregion

        #region Unit Tests

        #region Login

        [Fact]
        public async Task Login_Should_ReturnTokenResponseAsync_WithValidCredentials()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>())).ReturnsAsync(GetUser());

            var authenticationService = new AuthenticationService(_jwtProvider, _userRepositoryMock.Object, _passwordHashChecker);

            // Act
            var testResult = await authenticationService.Login("john.doe@test.com", @"John@123");

            // Assert
            testResult.Should().NotBeNull();
            testResult.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_Should_ThrowDomainException_WithInvalidEmail()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>())).ReturnsAsync((User)default);

            var authenticationService = new AuthenticationService(_jwtProvider, _userRepositoryMock.Object, _passwordHashChecker);

            // Act
            var action = () => authenticationService.Login("johndoe@test.com", @"John@123");

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Authentication.InvalidEmailOrPassword.Message);
        }

        [Fact]
        public async Task Login_Should_ThrowDomainException_WithInvalidPassword()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>())).ReturnsAsync(GetUser());

            var authenticationService = new AuthenticationService(_jwtProvider, _userRepositoryMock.Object, _passwordHashChecker);

            // Act
            var action = () => authenticationService.Login("john.doe@test.com", @"John@123456");

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Authentication.InvalidEmailOrPassword.Message);
        }

        #endregion

        #endregion

        #region Private Methods

        private UserTest GetUser() => new UserTest
        (
            idUser: 1,
            name: "John",
            cpf: "41548568040",
            crm: "4154856-BR",
            email: Email.Create("john.doe@test.com"),
            userRole: UserRoles.Doctor,
            passwordHash: new PasswordHasher().HashPassword(Password.Create("John@123"))
        );

        private IOptions<JwtSettings> JwtOptions => Options.Create<JwtSettings>(new JwtSettings
        {
            Issuer = "http://localhost",
            Audience = "http://localhost",
            SecurityKey = "f143bfc760543ec317abd4e8748d9f2b44dfb07a",
            TokenExpirationInMinutes = 60
        });

        #endregion
    }
}
