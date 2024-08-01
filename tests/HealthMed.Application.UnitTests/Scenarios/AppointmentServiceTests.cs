using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HealthMed.Application.Contracts.Appointment;
using HealthMed.Application.Core.Abstractions.Cryptography;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Core.Abstractions.Messaging;
using HealthMed.Application.Services;
using HealthMed.Application.UnitTests.TestEntities;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Extensions;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.ValueObjects;
using HealthMed.Infrastructure.Cryptography;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;
using Enums = HealthMed.Domain.Enumerations;

namespace HealthMed.Application.UnitTests.Scenarios
{
    public sealed class AppointmentServiceTests
    {
        #region Read-Only Fields

        private readonly IPasswordHasher _passwordHasher;

        private readonly Mock<IDbContext> _dbContextMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;

        #endregion

        #region Constructors

        public AppointmentServiceTests()
        {
            _dbContextMock = new();
            _unitOfWorkMock = new();
            _mailServiceMock = new();
            _userRepositoryMock = new();
            _appointmentRepositoryMock = new();

            _passwordHasher = new PasswordHasher();
        }

        #endregion

        #region Unit Tests

        #region ListAsync

        [Fact]
        public async Task ListAsync_Should_ThrowNotFoundException_WhenInvalidUserDoctor()
        {
            // Arrange
            var userDoctor = DoctorB;
            var fromDate = DateTime.Now.WithoutSeconds();

            var expectedAppointmentList = AppointmentsList()
                .Where(x => x.IdDoctor == userDoctor.Id && x.AppointmentDate >= fromDate && x.IdAppointmentStatus != (byte)Enums.AppointmentStatus.Canceled);

            _dbContextMock.Setup(x => x.Set<Appointment, int>()).ReturnsDbSet(AppointmentsList());
            _dbContextMock.Setup(x => x.Set<AppointmentStatus, byte>()).ReturnsDbSet(AppointmentStatusList());
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            var appointmentService = new AppointmentService(_dbContextMock.Object, _unitOfWorkMock.Object,
                _mailServiceMock.Object, _userRepositoryMock.Object, _appointmentRepositoryMock.Object);

            // Act
            var action = () => appointmentService.ListAsync(new ListAppointmentRequest(page: 1, pageSize: 100), userDoctor.Id, fromDate: null);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ListAsync_Should_ReturnAppointmentResponsePageList_WhenValidParameters()
        {
            // Arrange
            var userDoctor = DoctorA;
            var fromDate = DateTime.Now.WithoutSeconds();

            var expectedAppointmentList = AppointmentsList()
                .Where(x => x.IdDoctor == userDoctor.Id && x.AppointmentDate >= fromDate && x.IdAppointmentStatus != (byte)Enums.AppointmentStatus.Canceled);

            _dbContextMock.Setup(x => x.Set<Appointment, int>()).ReturnsDbSet(AppointmentsList());            
            _dbContextMock.Setup(x => x.Set<AppointmentStatus, byte>()).ReturnsDbSet(AppointmentStatusList());
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(userDoctor);

            var appointmentService = new AppointmentService(_dbContextMock.Object, _unitOfWorkMock.Object,
                _mailServiceMock.Object, _userRepositoryMock.Object, _appointmentRepositoryMock.Object);

            // Act
            var testResult = await appointmentService.ListAsync(new ListAppointmentRequest(page: 1, pageSize: 100), userDoctor.Id, fromDate);

            // Assert
            testResult.Should().NotBeNull();
            testResult.Page.Should().Be(1);
            testResult.PageSize.Should().Be(100);
            testResult.Items.IsNullOrEmpty().Should().BeFalse();
            testResult.TotalCount.Should().Be(expectedAppointmentList.Count());

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ListAsync_Should_ReturnAppointmentResponsePageList_WhenNullFromDateParameter()
        {
            // Arrange
            var userDoctor = DoctorA;            

            var expectedAppointmentList = AppointmentsList()
                .Where(x => x.IdDoctor == userDoctor.Id && x.IdAppointmentStatus != (byte)Enums.AppointmentStatus.Canceled);

            _dbContextMock.Setup(x => x.Set<Appointment, int>()).ReturnsDbSet(AppointmentsList());
            _dbContextMock.Setup(x => x.Set<AppointmentStatus, byte>()).ReturnsDbSet(AppointmentStatusList());
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(userDoctor);

            var appointmentService = new AppointmentService(_dbContextMock.Object, _unitOfWorkMock.Object,
                _mailServiceMock.Object, _userRepositoryMock.Object, _appointmentRepositoryMock.Object);

            // Act
            var testResult = await appointmentService.ListAsync(new ListAppointmentRequest(page: 1, pageSize: 100), userDoctor.Id, fromDate: null);

            // Assert
            testResult.Should().NotBeNull();
            testResult.Page.Should().Be(1);
            testResult.PageSize.Should().Be(100);
            testResult.Items.IsNullOrEmpty().Should().BeFalse();
            testResult.TotalCount.Should().Be(expectedAppointmentList.Count());

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ListAsync_Should_ReturnNullAppointmentResponsePageList_WhenWithoutDoctorSchedule()
        {
            // Arrange
            var userDoctor = DoctorB;
            var fromDate = DateTime.Now.WithoutSeconds();

            var expectedAppointmentList = AppointmentsList()
                .Where(x => x.IdDoctor == userDoctor.Id && x.AppointmentDate >= fromDate && x.IdAppointmentStatus != (byte)Enums.AppointmentStatus.Canceled);

            _dbContextMock.Setup(x => x.Set<Appointment, int>()).ReturnsDbSet(AppointmentsList());
            _dbContextMock.Setup(x => x.Set<AppointmentStatus, byte>()).ReturnsDbSet(AppointmentStatusList());
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(userDoctor);

            var appointmentService = new AppointmentService(_dbContextMock.Object, _unitOfWorkMock.Object,
                _mailServiceMock.Object, _userRepositoryMock.Object, _appointmentRepositoryMock.Object);

            // Act
            var testResult = await appointmentService.ListAsync(new ListAppointmentRequest(page: 1, pageSize: 100), userDoctor.Id, fromDate);

            // Assert
            testResult.Should().NotBeNull();
            testResult.Page.Should().Be(1);
            testResult.PageSize.Should().Be(100);
            testResult.Items.IsNullOrEmpty().Should().BeTrue();
            testResult.TotalCount.Should().Be(expectedAppointmentList.Count());

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #endregion

        #region Private Methods

        private IEnumerable<Appointment> AppointmentsList()
        {
            // yesterday's appointment
            yield return AppointmentTest.Create(30000, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(-1).AddHours(8));
            yield return AppointmentTest.Create(30001, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(-1).AddHours(9));
            yield return AppointmentTest.Create(30002, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(-1).AddHours(10));
            yield return AppointmentTest.Create(30003, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(-1).AddHours(11));

            // today's appointment
            yield return AppointmentTest.Create(30004, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(8));
            yield return AppointmentTest.Create(30005, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(9));
            yield return AppointmentTest.Create(30006, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(10), userPatient: PatientA);
            yield return AppointmentTest.Create(30007, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(11), userPatient: PatientA);
            yield return AppointmentTest.Create(30008, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(12), userPatient: PatientA, toCanceled: true);
            yield return AppointmentTest.Create(30009, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(15));
            yield return AppointmentTest.Create(30010, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(16));
            yield return AppointmentTest.Create(30011, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(17), userPatient: PatientB);
            yield return AppointmentTest.Create(30012, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(18), userPatient: PatientB);
            yield return AppointmentTest.Create(30013, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(19));
            yield return AppointmentTest.Create(30014, userDoctor: DoctorA, appointmentDate: DateTime.Now.Date.AddDays(1).AddHours(20));
        }

        private IEnumerable<AppointmentStatus> AppointmentStatusList()
        {
            yield return new AppointmentStatus(idAppointmentStatus: (byte)Enums.AppointmentStatus.Available, name: "Disponível");
            yield return new AppointmentStatus(idAppointmentStatus: (byte)Enums.AppointmentStatus.Busy, name: "Ocupado");
            yield return new AppointmentStatus(idAppointmentStatus: (byte)Enums.AppointmentStatus.Canceled, name: "Cancelado");
        }

        private UserTest DoctorA
            => new UserTest(10000, "DoctorA", "16058582008", "1605858-BR", Email.Create("doctora@healthmed.app"), Enums.UserRoles.Doctor, _passwordHasher.HashPassword(Password.Create("Doctor@123")));

        private UserTest DoctorB
            => new UserTest(10001, "DoctorB", "16058582008", "1605858-BR", Email.Create("doctorB@healthmed.app"), Enums.UserRoles.Doctor, _passwordHasher.HashPassword(Password.Create("Doctor@123")));

        private UserTest PatientA
            => new UserTest(20000, "PatientA", "04953908015", null, Email.Create("patienta@test.com"), Enums.UserRoles.Patient, _passwordHasher.HashPassword(Password.Create("Patient@123")));
        private UserTest PatientB
            => new UserTest(20001, "PatientB", "30050470086", null, Email.Create("patientb@test.com"), Enums.UserRoles.Patient, _passwordHasher.HashPassword(Password.Create("Patient@123")));

        #endregion
    }
}
