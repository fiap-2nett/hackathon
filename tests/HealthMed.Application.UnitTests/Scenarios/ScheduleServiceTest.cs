using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Services;
using HealthMed.Application.UnitTests.TestEntities;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.ValueObjects;
using HealthMed.Infrastructure.Cryptography;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HealthMed.Application.UnitTests.Scenarios
{
    public sealed class ScheduleServiceTest
    {
        #region Read-Only Fields

        private readonly IDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        private readonly Mock<IDbContext> _dbContextMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly SchedulesService _schedulesService;

        #endregion

        #region Constructors

        public ScheduleServiceTest()
        {
            _dbContextMock = new();
            _unitOfWorkMock = new();
            _scheduleRepositoryMock = new();
            _userRepositoryMock = _userRepositoryMock = new();
            _appointmentRepositoryMock = new();

            _schedulesService = new SchedulesService(
                _dbContextMock.Object,
                _unitOfWorkMock.Object,
                _scheduleRepositoryMock.Object,
                _userRepositoryMock.Object,
                _appointmentRepositoryMock.Object
            );
        }

        #endregion

        #region Unit Tests

        #region Create

        /// <summary>
        /// Teste: Criação bem-sucedida de horários para um médico válido
        /// </summary>
        [Fact]
        public async Task CreateAsync_Should_Create_Schedules_For_Valid_Doctor()
        {
            var schedules = GetDynamicSchedulesValid();
            var user = GetUserDoctor();
            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _scheduleRepositoryMock.Setup(x => x.HasScheduleConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());

            // Act
            var result = await _schedulesService.CreateAsync(user.Id, schedules);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(schedules.Count, result.Count);
        }

        /// <summary>
        /// Teste: Erro ao criar horários para um usuário não encontrado
        /// </summary>
        [Fact]
        public async Task CreateAsync_Should_Throw_NotFoundException_For_NonExistent_User()
        {
            // Arrange
            var userId = 1;
            var schedules = GetDynamicSchedulesValid();
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var message = await Assert.ThrowsAsync<NotFoundException>(() => _schedulesService.CreateAsync(userId, schedules));
            message.Equals("The user with the specified identifier was not found.");
        }

        /// <summary>
        /// Teste: Erro ao criar horários para um usuário sem permissões
        /// </summary>
        [Fact]
        public async Task CreateAsync_Should_Throw_InvalidPermissionException_For_NonDoctor_User()
        {
            // Arrange
            var schedules = GetDynamicSchedulesValid();
            var user = GetUserPatient();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Act & Assert
            var message = await Assert.ThrowsAsync<InvalidPermissionException>(() => _schedulesService.CreateAsync(user.Id, schedules));
            message.Equals("The current user does not have the permissions to perform that operation.");
        }

        /// <summary>
        /// Teste: Erro ao criar horários inválidos
        /// </summary>
        [Fact]
        public async Task CreateAsync_Should_Throw_DomainException_For_Invalid_Schedules()
        {
            // Arrange
            var schedules = GetDynamicSchedulesInvalid();
            var user = GetUserDoctor();
            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Act & Assert
            var message = await Assert.ThrowsAsync<DomainException>(() => _schedulesService.CreateAsync(user.Id, schedules));
            message.Equals("Invalid schedule. Check for overlapping times, broken schedules, or incorrect duration.");
        }

        /// <summary>
        /// Teste: Erro ao criar horários com conflito
        /// </summary>
        [Fact]
        public async Task CreateAsync_Should_Throw_DomainException_For_Conflicting_Schedules()
        {
            //Arrange
            var schedules = GetDynamicSchedulesInvalid();
            var user = GetUserDoctor();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _scheduleRepositoryMock.Setup(x => x.HasScheduleConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);

            // Act & Assert
            var message = await Assert.ThrowsAsync<DomainException>(() => _schedulesService.CreateAsync(user.Id, schedules));
            message.Equals("There is a conflicting appointment in the specified period.");
        }

        #endregion

        #region Update



        #endregion

        #endregion

        #region Private Methods
        private ScheduleTest GetScheduleValid() => new ScheduleTest
        (
            idSchedule: 1,
            idDoctor: 1,
            startDate: DateTime.UtcNow.AddHours(1),
            endDate: DateTime.UtcNow.AddHours(3)
        );
        private DateTime RoundToNearestHour(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
        }
        private List<dynamic> GetDynamicSchedulesValid()
        {
            return new List<dynamic>
            {
                new  ScheduleDto { StartDate = RoundToNearestHour(DateTime.Now.AddHours(1)), EndDate = RoundToNearestHour(DateTime.Now.AddHours(2)) },
                new  ScheduleDto { StartDate = RoundToNearestHour(DateTime.Now.AddHours(3)), EndDate = RoundToNearestHour(DateTime.Now.AddHours(4)) }
            };
        }
        private List<dynamic> GetDynamicSchedulesInvalid()
        {
            return new List<dynamic>
            {
                new  ScheduleDto { StartDate = RoundToNearestHour(DateTime.Now.AddHours(-1)), EndDate = RoundToNearestHour(DateTime.Now.AddHours(2)) },
                new  ScheduleDto { StartDate = RoundToNearestHour(DateTime.Now.AddHours(-3)), EndDate = RoundToNearestHour(DateTime.Now.AddHours(4)) }
            };
        }
        private UserTest GetUserDoctor() => new UserTest
        (
            idUser: 1,
            name: "John",
            cpf: "41548568040",
            crm: "4154856-BR",
            email: Email.Create("john.doe@test.com"),
            userRole: UserRoles.Doctor,
            passwordHash: new PasswordHasher().HashPassword(Password.Create("John@123"))
        );

        private UserTest GetUserPatient() => new UserTest
        (
            idUser: 2,
            name: "John silva",
            cpf: "41548568040",
            crm: "4154856-BR",
            email: Email.Create("john_silva.doe@test.com"),
            userRole: UserRoles.Patient,
            passwordHash: new PasswordHasher().HashPassword(Password.Create("John@123"))
        );

        #endregion
    }
}
