using System;
using System.Linq;
using System.Threading.Tasks;
using HealthMed.Application.Contracts.Appointment;
using HealthMed.Application.Contracts.Common;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Enums = HealthMed.Domain.Enumerations;

namespace HealthMed.Application.Services
{
    internal sealed class AppointmentService : IAppointmentService
    {
        #region Read-Only Fields

        private readonly IDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        #endregion

        #region Constructors

        public AppointmentService(
            IDbContext dbContext,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IAppointmentRepository appointmentRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        }

        #endregion

        #region IAppointmentService Members

        public async Task<PagedList<AppointmentResponse>> ListAsync(ListAppointmentRequest listAppointmentRequest, int idUserDoctor, DateTime? fromDate)
        {
            var userDoctor = await _userRepository.GetByIdAsync(idUserDoctor);
            if (userDoctor is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            IQueryable<AppointmentResponse> appointmentsQuery = (
                from appointment in _dbContext.Set<Appointment, int>().AsNoTracking()
                join status in _dbContext.Set<AppointmentStatus, byte>().AsNoTracking()
                    on appointment.IdAppointmentStatus equals status.Id
                where
                    appointment.IdDoctor == idUserDoctor &&
                    appointment.IdAppointmentStatus != (byte)Enums.AppointmentStatus.Canceled
                orderby
                    appointment.AppointmentDate
                select new AppointmentResponse
                {
                    IdAppointment = appointment.Id,
                    Date = appointment.AppointmentDate,
                    Status = new AppointmentStatusResponse { IdAppointmentStatus = status.Id, Name = status.Name }
                }
            );

            if (fromDate.HasValue)
                appointmentsQuery = appointmentsQuery.Where(x => x.Date >= fromDate.GetValueOrDefault());

            var totalCount = await appointmentsQuery.CountAsync();

            var appointmentsReponsePage = await appointmentsQuery
                .Skip((listAppointmentRequest.Page - 1) * listAppointmentRequest.PageSize)
                .Take(listAppointmentRequest.PageSize)
                .ToArrayAsync();

            return new PagedList<AppointmentResponse>(appointmentsReponsePage, listAppointmentRequest.Page, listAppointmentRequest.PageSize, totalCount);
        }

        public async Task ReserveAsync(int idAppointment, int idUserDoctor, int idUserPatient)
        {
            try
            {
                var userDoctor = await _userRepository.GetByIdAsync(idUserDoctor);
                if (userDoctor is null)
                    throw new NotFoundException(DomainErrors.User.NotFound);

                var userPatient = await _userRepository.GetByIdAsync(idUserPatient);
                if (userPatient is null)
                    throw new NotFoundException(DomainErrors.User.NotFound);

                var appointment = await _appointmentRepository.GetByIdAsync(idAppointment);
                if (appointment is null)
                    throw new NotFoundException(DomainErrors.Appointment.NotFound);

                if (await _appointmentRepository.IsOverlappingAsync(userPatient.Id, appointment.AppointmentDate))
                    throw new DomainException(DomainErrors.Appointment.Overlap);

                appointment.Reserve(userPatient);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DomainException(DomainErrors.Appointment.Overlap);
            }
        }

        #endregion
    }
}
