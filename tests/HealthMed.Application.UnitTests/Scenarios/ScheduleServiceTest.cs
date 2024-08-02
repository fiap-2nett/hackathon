using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Services;
using HealthMed.Application.UnitTests.TestEntities;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.ValueObjects;
using HealthMed.Domain.Extensions;
using HealthMed.Infrastructure.Cryptography;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                _userRepositoryMock.Object,
                _scheduleRepositoryMock.Object,
                _appointmentRepositoryMock.Object
            );
        }

        #endregion

        #region Unit Tests

        #region Get

        /// <summary>
        /// Teste: Recuperar uma lista paginada de horários quando existem horários cadastrados
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnPagedList_WhenSchedulesExist()
        {
            // Arrange
            var schedules = GetListSchedule().AsQueryable();
            _dbContextMock.Setup(x => x.Set<Schedule, int>()).ReturnsDbSet(schedules);

            // Act
            var result = await _schedulesService.GetAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
        }

        /// <summary>
        /// Teste: Recuperar uma lista paginada vazia quando não existem horários cadastrados
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnEmptyPagedList_WhenNoSchedulesExist()
        {
            // Arrange
            var schedules = new List<Schedule>().AsQueryable();
            _dbContextMock.Setup(x => x.Set<Schedule, int>()).ReturnsDbSet(schedules);

            // Act
            var result = await _schedulesService.GetAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        /// <summary>
        /// Teste: Recuperar detalhes de um horário quando o horário existe
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ShouldReturnDetailedScheduleResponse_WhenScheduleExists()
        {
            // Arrange
            var schedule = GetScheduleValid();
            var user = GetUserDoctor();

            var schedules = new List<Schedule> { schedule }.AsQueryable();
            var users = new List<User> { user }.AsQueryable();

            _dbContextMock.Setup(x => x.Set<Schedule, int>()).ReturnsDbSet(schedules);
            _dbContextMock.Setup(x => x.Set<User, int>()).ReturnsDbSet(users);

            // Act
            var result = await _schedulesService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John", result.Doctor.Name);
        }

        /// <summary>
        /// Teste: Retornar nulo quando o horário não existe
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenScheduleDoesNotExist()
        {
            // Arrange
            var schedules = new List<Schedule>().AsQueryable();
            var users = new List<User>().AsQueryable();

            _dbContextMock.Setup(x => x.Set<Schedule, int>()).ReturnsDbSet(schedules);
            _dbContextMock.Setup(x => x.Set<User, int>()).ReturnsDbSet(users);

            // Act
            var result = await _schedulesService.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        #endregion

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

        /// <summary>
        /// Teste: Atualização bem-sucedida de um agendamento existente
        /// </summary>
        [Fact]
        public async Task UpdateAsync_Should_Update_Schedule_Successfully()
        {
            // Arrange
            var user = GetUserDoctor();
            var schedule = GetScheduleValid();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _scheduleRepositoryMock.Setup(x => x.GetByIdAsync(schedule.Id)).ReturnsAsync(schedule);
            _scheduleRepositoryMock.Setup(x => x.HasScheduleConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);
            _appointmentRepositoryMock.Setup(x => x.GetByDoctorAndDateAsync(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync((Appointment)null);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never());

            // Act
            var result = await _schedulesService.Update(user.Id, schedule.IdDoctor, schedule.StartDate, schedule.EndDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(schedule.Id, result.Id);
            Assert.Equal(schedule.StartDate, result.StartDate);
            Assert.Equal(schedule.EndDate, result.EndDate);
        }

        /// <summary>
        /// Teste: Erro ao atualizar um agendamento para um usuário não encontrado
        /// </summary>
        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_For_NonExistent_User()
        {
            // Arrange
            var userId = 1;
            var scheduleId = 1;
            var startDate = DateTime.Now.AddHours(1);
            var endDate = DateTime.Now.AddHours(2);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var messsage = await Assert.ThrowsAsync<NotFoundException>(() => _schedulesService.Update(userId, scheduleId, startDate.WithoutSeconds(), endDate.WithoutSeconds()));
            messsage.Equals("The user with the specified identifier was not found.");
        }

        /// <summary>
        /// Teste: Erro ao atualizar um agendamento que não existe
        /// </summary>
        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_For_NonExistent_Schedule()
        {
            // Arrange
            var scheduleId = 1;
            var startDate = DateTime.Now.AddHours(1);
            var endDate = DateTime.Now.AddHours(2);
            var user = GetUserDoctor();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _scheduleRepositoryMock.Setup(x => x.GetByIdAsync(scheduleId)).ReturnsAsync((Schedule)null);

            // Act & Assert
            var message = await Assert.ThrowsAsync<NotFoundException>(() => _schedulesService.Update(user.Id, scheduleId, startDate.WithoutSeconds(), endDate.WithoutSeconds()));
            message.Equals("The schedule with the specified identifier was not found.");
        }

        /// <summary>
        /// Teste: Erro ao atualizar um agendamento com horários inválidos
        /// </summary>
        [Fact]
        public async Task UpdateAsync_Should_Throw_DomainException_For_Invalid_Schedule()
        {
            // Arrange
            var startDate = DateTime.Now.AddHours(-1); // Início no passado
            var endDate = DateTime.Now.AddHours(2);
            var user = GetUserDoctor();
            var schedule = GetScheduleInvalid();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _scheduleRepositoryMock.Setup(x => x.GetByIdAsync(schedule.Id)).ReturnsAsync(schedule);

            // Act & Assert
            var message = await Assert.ThrowsAsync<DomainException>(() => _schedulesService.Update(user.Id, schedule.Id, startDate.WithoutSeconds(), endDate.WithoutSeconds()));
            message.Equals("Invalid schedule. Check for overlapping times, broken schedules, or incorrect duration.");
        }

        /// <summary>
        /// Teste: Erro ao atualizar com conflito de horário
        /// </summary>
        [Fact]
        public async Task UpdateAsync_Should_Throw_DomainException_For_Schedule_Conflict()
        {
            // Arrange
            var dateNow = DateTime.Now;
            var userId = 1;
            var scheduleId = 1;
            var startDate = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, 15, 0);
            var endDate = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.AddHours(2).Hour, 0, 0);
            var user = GetUserDoctor();
            var schedule = GetScheduleInvalid();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _scheduleRepositoryMock.Setup(x => x.GetByIdAsync(scheduleId)).ReturnsAsync(schedule);

            // Act & Assert
            var message = await Assert.ThrowsAsync<DomainException>(() => _schedulesService.Update(userId, scheduleId, startDate.WithoutSeconds(), endDate.WithoutSeconds()));
            message.Equals("There is a conflicting appointment in the specified period.");
        }


        #endregion

        #endregion

        #region Private Methods

        private List<ScheduleTest> GetListSchedule()
        {
            return new List<ScheduleTest>
            {
                new ScheduleTest(1, 1, DateTime.Now, DateTime.Now.AddHours(1)),
                new ScheduleTest(2, 1, DateTime.Now.AddHours(2), DateTime.Now.AddHours(3))
            };
        }
        private ScheduleTest GetScheduleValid() => new ScheduleTest
        (
            idSchedule: 1,
            idDoctor: 1,
            startDate: DateTime.Now.AddHours(1),
            endDate: DateTime.Now.AddHours(3)
        );
        private ScheduleTest GetScheduleInvalid() => new ScheduleTest
        (
            idSchedule: 2,
            idDoctor: 1,
            startDate: DateTime.Now.AddHours(-1),
            endDate: DateTime.Now.AddHours(3)
        );
        private List<(DateTime StartDate, DateTime EndDate)> GetDynamicSchedulesValid()
        {
            return new List<(DateTime StartData, DateTime EndDate)>
            {
                (DateTime.Now.AddHours(1).WithoutSeconds(), DateTime.Now.AddHours(2).WithoutSeconds()),
                (DateTime.Now.AddHours(3).WithoutSeconds(), DateTime.Now.AddHours(4).WithoutSeconds())
            };
        }
        private List<(DateTime StartDate, DateTime EndDate)> GetDynamicSchedulesInvalid()
        {
            return new List<(DateTime StartDate, DateTime EndDate)>
            {
                (DateTime.Now.AddHours(-1).WithoutSeconds(), DateTime.Now.AddHours(2).WithoutSeconds()),
                (DateTime.Now.AddHours(-3).WithoutSeconds(), DateTime.Now.AddHours(4).WithoutSeconds())
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
