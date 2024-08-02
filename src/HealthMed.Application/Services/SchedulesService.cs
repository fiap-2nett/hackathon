using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthMed.Application.Contracts.Common;
using HealthMed.Application.Contracts.Schedule;
using HealthMed.Application.Contracts.User;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Extensions;
using HealthMed.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HealthMed.Application.Services
{
    internal sealed class SchedulesService : IScheduleService
    {
        #region Read-Only Fields

        private readonly IDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        #endregion

        #region Constructors

        public SchedulesService(IDbContext dbContext,
                                IUnitOfWork unitOfWork,
                                IUserRepository userRepository,
                                IScheduleRepository scheduleService,
                                IAppointmentRepository appointmentRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
            _userRepository = userRepository ?? throw new ArgumentException(nameof(userRepository));
            _scheduleRepository = scheduleService ?? throw new ArgumentException(nameof(scheduleService));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentException(nameof(appointmentRepository));
        }

        #endregion

        #region ISchedulesService Members

        public async Task<PagedList<ScheduleResponse>> GetAsync(int page, int pageSize)
        {
            IQueryable<ScheduleResponse> scheduleQuery = (
                from schedule in _dbContext.Set<Schedule, int>().AsNoTracking()
                select new ScheduleResponse
                {
                    Id = schedule.Id,
                    StartDate = schedule.StartDate,
                    EndDate = schedule.EndDate,
                });

            var totalCount = await scheduleQuery.CountAsync();

            var scheduleReponsePage = await scheduleQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync();

            return new PagedList<ScheduleResponse>(scheduleReponsePage, page, pageSize, totalCount);
        }

        public async Task<DetailedScheduleResponse> GetByIdAsync(int idSchedule)
        {
            IQueryable<DetailedScheduleResponse> scheduleQuery = (
                      from schedule in _dbContext.Set<Schedule, int>().AsNoTracking()
                      join user in _dbContext.Set<User, int>().AsNoTracking()
                          on schedule.IdDoctor equals user.Id
                      where
                          schedule.Id == idSchedule
                      select new DetailedScheduleResponse
                      {
                          Id = schedule.Id,
                          StartDate = schedule.StartDate,
                          EndDate = schedule.EndDate,
                          Doctor = new UserResponse { Id = user.Id, Name = user.Name }
                      }
                  );

            return await scheduleQuery.FirstOrDefaultAsync();
        }

        public async Task<List<ScheduleResponse>> CreateAsync(int idUserPerformedAction, List<(DateTime StartDate, DateTime EndDate)> intervals)
        {
            var userDoctor = await _userRepository.GetByIdAsync(idUserPerformedAction);
            if (userDoctor is null)
                throw new NotFoundException(DomainErrors.User.NotFound);            

            if (!Schedule.IsValidSchedule(intervals))
                throw new DomainException(DomainErrors.Schedule.ScheduleInvalid);

            await VerifyPeriod(userDoctor.Id, intervals);
            var scheduleList = Schedule.CreateSchedules(userDoctor, intervals);

            _scheduleRepository.InsertRange(scheduleList);
            _appointmentRepository.InsertRange(Appointment.BuildAppointmentListFromSchedules(userDoctor.Id, scheduleList));

            await _unitOfWork.SaveChangesAsync();

            return scheduleList
                .Select(s => new ScheduleResponse { Id = s.Id, StartDate = s.StartDate, EndDate = s.EndDate })
                .ToList();
        }

        public async Task<ScheduleResponse> Update(int idUserPerformedAction, int idSchedule, DateTime startDate, DateTime endDate)
        {
            var userDoctor = await _userRepository.GetByIdAsync(idUserPerformedAction);            
            if (userDoctor is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            if (userDoctor.IdRole != (int)UserRoles.Doctor)
                throw new InvalidPermissionException(DomainErrors.Schedule.InvalidPermissions);

            var schedule = await _scheduleRepository.GetByIdAsync(idSchedule);
            if (schedule is null)
                throw new NotFoundException(DomainErrors.Schedule.NotFound);

            if (!Schedule.IsValidSchedule(startDate.WithoutSeconds(), endDate.WithoutSeconds()))
                throw new DomainException(DomainErrors.Schedule.ScheduleInvalid);

            if (await _scheduleRepository.HasScheduleConflictAsync(userDoctor.Id, idSchedule, startDate.WithoutSeconds(), endDate))
                throw new DomainException(DomainErrors.Schedule.Conflicting);            

            var newAppointments = Appointment.BuildAppointmentListFromSchedules(userDoctor.Id, startDate.WithoutSeconds(), endDate.WithoutSeconds());
            var oldAppointments = Appointment.BuildAppointmentListFromSchedules(userDoctor.Id, schedule.StartDate.WithoutSeconds(), schedule.EndDate.WithoutSeconds());
                       
            schedule.Update(startDate.WithoutSeconds(), endDate.WithoutSeconds(), userDoctor);
            await UpdateAppointmentsAsync(newAppointments, oldAppointments);
            
            await _unitOfWork.SaveChangesAsync();

            return new ScheduleResponse
            {
                Id = schedule.Id,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate
            };
        }

        #endregion

        #region Private Methods

        private async Task UpdateAppointmentsAsync(List<Appointment> newAppointments, List<Appointment> oldAppointments)
        {
            foreach (var newAppointment in newAppointments)
            {
                var existingAppointment = await _appointmentRepository.GetByDoctorAndDateAsync(newAppointment.IdDoctor, newAppointment.AppointmentDate);

                if (existingAppointment is null)
                {
                    _appointmentRepository.Insert(newAppointment);
                }
                else
                {
                    if (existingAppointment.IdPatient.HasValue)
                    {
                        existingAppointment.Cancel();
                        _appointmentRepository.Insert(newAppointment);
                    }
                    else
                    {
                        existingAppointment.ChangeLastUpdateDate();
                    }
                }

                oldAppointments.RemoveAll(a => a.IdDoctor == newAppointment.IdDoctor && a.AppointmentDate == newAppointment.AppointmentDate);
            }

            foreach (var oldAppointment in oldAppointments)
            {
                var existingAppointment = await _appointmentRepository.GetByDoctorAndDateAsync(oldAppointment.IdDoctor, oldAppointment.AppointmentDate);
                if (existingAppointment is null)
                    continue;
                
                existingAppointment.Cancel();                
            }
        }

        private async Task VerifyPeriod(int userId, List<(DateTime StartDate, DateTime EndDate)> intervals)
        {
            foreach (var interval in intervals)
            {                
                if (await _scheduleRepository.HasScheduleConflictAsync(userId, interval.StartDate.WithoutSeconds(), interval.EndDate.WithoutSeconds()))
                    throw new DomainException(DomainErrors.Schedule.Conflicting);
            }
        }

        #endregion
    }
}
