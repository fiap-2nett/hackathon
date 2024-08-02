using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Services;
using HealthMed.Application.UnitTests.TestEntities;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Extensions;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.ValueObjects;
using HealthMed.Infrastructure.Cryptography;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using static System.Collections.Specialized.BitVector32;

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
            var result = await _schedulesService.GetByIdAsync(schedule.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(schedule.Id);
            result.Doctor.Name.Should().Be(user.Name);

            _dbContextMock.Verify(x => x.Set<Schedule, int>(), Times.Once);
            _dbContextMock.Verify(x => x.Set<User, int>(), Times.Once);
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
            result.Should().BeNull();
        }

        #endregion

        #region Create

        /// <summary>
        /// Teste: Criação bem-sucedida de horários para um médico válido
        /// </summary>
        [Fact]
        public async Task CreateAsync_Should_Create_Schedules_For_Valid_Doctor()
        {
            // Arrange
            var schedules = GetDynamicSchedulesValid();
            var user = GetUserDoctor();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _scheduleRepositoryMock.Setup(x => x.HasScheduleConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);

            // Act
            var result = await _schedulesService.CreateAsync(user.Id, schedules);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(schedules.Count);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.HasScheduleConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Exactly(schedules.Count));
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
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

            // Act
            var action = () => _schedulesService.CreateAsync(userId, schedules);

            // Assert
            await action.Should()
                        .ThrowAsync<NotFoundException>()
                        .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
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

            // Act
            var action =  () => _schedulesService.CreateAsync(user.Id, schedules);

            // Assert
            await action.Should()
                        .ThrowAsync<InvalidPermissionException>()
                        .WithMessage(DomainErrors.Schedule.InvalidPermissions.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
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

            // Act
            var action = () => _schedulesService.CreateAsync(user.Id, schedules);

            // Assert
            await action.Should()
                        .ThrowAsync<DomainException>()
                        .WithMessage(DomainErrors.Schedule.ScheduleInvalid.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.HasScheduleConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        /// <summary>
        /// Teste: Erro ao criar horários com conflito
        /// </summary>
        [Fact]
        public async Task CreateAsync_Should_Throw_DomainException_For_Conflicting_Schedules()
        {
            // Arrange
            var schedules = GetDynamicSchedulesInvalid();
            var user = GetUserDoctor();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _scheduleRepositoryMock.Setup(x => x.HasScheduleConflictAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);

            // Act
            var action = () => _schedulesService.CreateAsync(user.Id, schedules);

            // Assert
            await action.Should()
                        .ThrowAsync<DomainException>()
                        .WithMessage(DomainErrors.Schedule.ScheduleInvalid.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
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
            _appointmentRepositoryMock.Setup(x => x.GetByDoctorAndDateAsync(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync((Domain.Entities.Appointment)null);

            // Act
            var result = await _schedulesService.Update(user.Id, schedule.IdDoctor, schedule.StartDate, schedule.EndDate);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(schedule.Id);
            result.StartDate.Should().Be(schedule.StartDate);
            result.EndDate.Should().Be(schedule.EndDate);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.GetByIdAsync(schedule.Id), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
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

            // Act
            var action = () => _schedulesService.Update(userId, scheduleId, startDate.WithoutSeconds(), endDate.WithoutSeconds());

            // Assert
            await action.Should()
                        .ThrowAsync<NotFoundException>()
                        .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
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

            // Act
            var action = () => _schedulesService.Update(user.Id, scheduleId, startDate.WithoutSeconds(), endDate.WithoutSeconds());

            // Assert
            await action.Should()
                        .ThrowAsync<NotFoundException>()
                        .WithMessage(DomainErrors.Schedule.NotFound.Message);
    
            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.GetByIdAsync(scheduleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
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

            // Act
            var action = () => _schedulesService.Update(user.Id, schedule.Id, startDate.WithoutSeconds(), endDate.WithoutSeconds());

            // Assert
            await action.Should()
                        .ThrowAsync<DomainException>()
                        .WithMessage(DomainErrors.Schedule.ScheduleInvalid.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.GetByIdAsync(schedule.Id), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
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
            _scheduleRepositoryMock.Setup(x => x.HasScheduleConflictAsync(userId, startDate, endDate)).ReturnsAsync(true);

            // Act
            var action = () => _schedulesService.Update(userId, scheduleId, startDate.WithoutSeconds(), endDate.WithoutSeconds());

            // Assert
            await action.Should()
                        .ThrowAsync<DomainException>()
                        .WithMessage(DomainErrors.Schedule.ScheduleInvalid.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _scheduleRepositoryMock.Verify(x => x.GetByIdAsync(scheduleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
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
