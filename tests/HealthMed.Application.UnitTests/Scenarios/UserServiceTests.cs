using System;
using System.Threading.Tasks;
using FluentAssertions;
using HealthMed.Application.Core.Abstractions.Authentication;
using HealthMed.Application.Core.Abstractions.Cryptography;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Services;
using HealthMed.Application.UnitTests.TestEntities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Repositories;
using HealthMed.Infrastructure.Authentication;
using HealthMed.Infrastructure.Authentication.Settings;
using HealthMed.Infrastructure.Cryptography;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using HealthMed.Domain.ValueObjects;
using HealthMed.Domain.Entities;

namespace HealthMed.Application.UnitTests.Scenarios
{
    public sealed class UserServiceTests
    {
        #region Read-Only Fields

        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;

        private readonly Mock<IDbContext> _dbContextMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        #endregion

        #region Constructors

        public UserServiceTests()
        {
            _dbContextMock = new();
            _unitOfWorkMock = new();
            _userRepositoryMock = new();

            _passwordHasher = new PasswordHasher();
            _jwtProvider = new JwtProvider(JwtOptions);
        }

        #endregion

        #region Unit Tests

        #region CreateAsync

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task CreateAsync_Should_ThrowArgumentException_WhenEmailIsNullOrWhiteSpace(string email)
        {
            // Arrange
            var userService = new UserService(_dbContextMock.Object, _unitOfWorkMock.Object, _jwtProvider,
                _userRepositoryMock.Object, _passwordHasher);

            // Act
            var action = () => userService.CreateAsync("John", "41548568040", "4154856-BR", email, UserRoles.Doctor, "John@123");

            // Assert
            await action.Should()
                .ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(email))
                .WithMessage(new ArgumentException(DomainErrors.Email.NullOrEmpty.Message, nameof(email)).Message);

            _userRepositoryMock.Verify(x => x.IsEmailUniqueAsync(It.IsAny<Email>()), Times.Never());
            _userRepositoryMock.Verify(x => x.Insert(It.IsAny<User>()), Times.Never());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());
        }

        [Fact]
        public async Task CreateAsync_Should_ThrowArgumentException_WhenEmailIsLongerThanAllowed()
        {
            // Arrange
            var email = $"{new string('a', Email.MaxLength)}@test.com";

            var userService = new UserService(_dbContextMock.Object, _unitOfWorkMock.Object, _jwtProvider,
                _userRepositoryMock.Object, _passwordHasher);

            // Act
            var action = () => userService.CreateAsync("John", "41548568040", "4154856-BR", email, UserRoles.Doctor, "John@123");

            // Assert
            await action.Should()
                .ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(email))
                .WithMessage(new ArgumentException(DomainErrors.Email.LongerThanAllowed.Message, nameof(email)).Message);

            _userRepositoryMock.Verify(x => x.IsEmailUniqueAsync(It.IsAny<Email>()), Times.Never());
            _userRepositoryMock.Verify(x => x.Insert(It.IsAny<User>()), Times.Never());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());
        }

        [Theory]
        [InlineData("john.doe@test")]
        [InlineData("john.doe@test.")]
        [InlineData("john.doe@test.a")]
        public async Task CreateAsync_Should_ThrowArgumentException_WhenEmailIsInvalidFormat(string email)
        {
            // Arrange
            var userService = new UserService(_dbContextMock.Object, _unitOfWorkMock.Object, _jwtProvider,
                _userRepositoryMock.Object, _passwordHasher);

            // Act
            var action = () => userService.CreateAsync("John", "41548568040", "4154856-BR", email, UserRoles.Doctor, "John@123");

            // Assert
            await action.Should()
                .ThrowAsync<ArgumentException>()
                .WithParameterName(nameof(email))
                .WithMessage(new ArgumentException(DomainErrors.Email.InvalidFormat.Message, nameof(email)).Message);

            _userRepositoryMock.Verify(x => x.IsEmailUniqueAsync(It.IsAny<Email>()), Times.Never());
            _userRepositoryMock.Verify(x => x.Insert(It.IsAny<User>()), Times.Never());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());
        }

        [Fact]
        public async Task CreateAsync_Should_ThrowDomainException_WhenEmailIsNotUnique()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>())).ReturnsAsync(false);

            var userService = new UserService(_dbContextMock.Object, _unitOfWorkMock.Object, _jwtProvider,
                _userRepositoryMock.Object, _passwordHasher);

            // Act
            var action = () => userService.CreateAsync("John", "41548568040", "4154856-BR", "john.doe@test.com", UserRoles.Doctor, "John@123");            

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.User.DuplicateEmail.Message);

            _userRepositoryMock.Verify(x => x.Insert(It.IsAny<User>()), Times.Never());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task CreateAsync_Should_ThrowDomainException_WhenRequiredValuesAreInvalid(string name)
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>())).ReturnsAsync(true);

            var userService = new UserService(_dbContextMock.Object, _unitOfWorkMock.Object, _jwtProvider,
                _userRepositoryMock.Object, _passwordHasher);

            // Act            
            var action = () => userService.CreateAsync(name, "41548568040", "4154856-BR", "john.doe@test.com", UserRoles.Doctor, "John@123");

            // Assert
            await action.Should()
                .ThrowAsync<ArgumentException>();

            _userRepositoryMock.Verify(x => x.Insert(It.IsAny<User>()), Times.Never());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("415485")]
        [InlineData("415485656A9S")]
        [InlineData("415g85-m!")]
        [InlineData("abcdef-24")]
        public async Task CreateAsync_Should_ThrowDomainException_WhenCRMAreInInvalidFormat(string crm)
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>())).ReturnsAsync(true);

            var userService = new UserService(_dbContextMock.Object, _unitOfWorkMock.Object, _jwtProvider,
                _userRepositoryMock.Object, _passwordHasher);

            // Act            
            var action = () => userService.CreateAsync("John", "41548568040", crm, "john.doe@test.com", UserRoles.Doctor, "John@123");

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.User.InvalidCRM.Message);

            _userRepositoryMock.Verify(x => x.Insert(It.IsAny<Domain.Entities.User>()), Times.Never());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());
        }

        [Theory]
        [InlineData("415485")]
        [InlineData("41548568040656")]
        [InlineData("4154856804A")]
        [InlineData("415$$568040")]
        [InlineData("00000000000")]
        [InlineData("12345678912")]
        public async Task CreateAsync_Should_ThrowDomainException_WhenCPFAreInInvalidFormat(string cpf)
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.IsEmailUniqueAsync(It.IsAny<Email>())).ReturnsAsync(true);

            var userService = new UserService(_dbContextMock.Object, _unitOfWorkMock.Object, _jwtProvider,
                _userRepositoryMock.Object, _passwordHasher);

            // Act            
            var action = () => userService.CreateAsync("John", cpf, "4154856-BR", "john.doe@test.com", UserRoles.Patient, "John@123");

            // Assert
            await action.Should()
                        .ThrowAsync<DomainException>()
                        .WithMessage(DomainErrors.User.InvalidCPF.Message);

            _userRepositoryMock.Verify(x => x.Insert(It.IsAny<Domain.Entities.User>()), Times.Never());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());
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
