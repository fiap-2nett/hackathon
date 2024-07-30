using System;
using System.Threading.Tasks;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.Entities;
using HealthMed.Application.Contracts.Schedule;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Enumerations;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HealthMed.Application.Contracts.User;
using HealthMed.Application.Contracts.Common;

namespace HealthMed.Application.Services
{
    internal sealed class SchedulesService : IScheduleService
    {
        #region Read-Only Fields

        private const int AppointmentDurationInMinutes = 60;
        private readonly IDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUserRepository _userRepository;

        #endregion

        #region Constructors

        public SchedulesService(IDbContext dbContext,
                                IUnitOfWork unitOfWork,
                                IScheduleRepository scheduleService,
                                IUserRepository userRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
            _scheduleRepository = scheduleService ?? throw new ArgumentException(nameof(scheduleService));
            _userRepository = userRepository ?? throw new ArgumentException(nameof(userRepository));

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
        public async Task<List<ScheduleResponse>> CreateAsync(int userId, IList<dynamic> schedules)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            if (user.IdRole != (int)UserRoles.Doctor)
                throw new InvalidPermissionException(DomainErrors.Schedule.InvalidPermissions);

            if (!IsValidSchedule(schedules))
                throw new DomainException(DomainErrors.Schedule.ScheduleInvalid);

            await VerifyPeriod(userId, schedules);

            var entities = Schedule.CreateSchedules(userId, schedules);

            _scheduleRepository.InsertRange(entities);
            await _unitOfWork.SaveChangesAsync();

            return entities.Select(s => new ScheduleResponse
            {
                Id = s.Id,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            }).ToList();
        }

        public async Task<ScheduleResponse> Update(int idUserPerformedAction, int scheduleId, DateTime startDate, DateTime endDate)
        {
            var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);

            if (userPerformedAction is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);

            if (schedule is null)
                throw new NotFoundException(DomainErrors.Schedule.NotFound);

            if (!IsValidSchedule(startDate, endDate))
                 throw new DomainException(DomainErrors.Schedule.ScheduleInvalid);

            if (await _scheduleRepository.HasScheduleConflictAsync(userPerformedAction.Id, scheduleId, startDate, endDate))
                    throw new DomainException(DomainErrors.Schedule.Conflicting);

            schedule.Update(startDate, endDate, userPerformedAction);

            _scheduleRepository.Update(schedule);
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
        private async Task VerifyPeriod(int userId, IList<dynamic> schedules)
        {
            foreach (var schedule in schedules)
            {
                DateTime startDate = new DateTime(schedule.StartDate.Year, schedule.StartDate.Month, schedule.StartDate.Day, schedule.StartDate.Hour, schedule.StartDate.Minute, 0);
                DateTime endDate = new DateTime(schedule.EndDate.Year, schedule.EndDate.Month, schedule.EndDate.Day, schedule.EndDate.Hour, schedule.EndDate.Minute, 0);

                if (await _scheduleRepository.HasScheduleConflictAsync(userId, startDate, endDate))
                    throw new DomainException(DomainErrors.Schedule.Conflicting);

            }
        }

        private bool IsValidSchedule(DateTime startDate, DateTime endDate)
        {
            DateTime currentDateTime = DateTime.Now;

            startDate = new DateTime(
                startDate.Year,
                startDate.Month,
                startDate.Day,
                startDate.Hour,
                startDate.Minute,
                0);

            endDate = new DateTime(
                endDate.Year,
                endDate.Month,
                endDate.Day,
                endDate.Hour,
                endDate.Minute,
                0);

            if (startDate < currentDateTime)
                return false;

            if (endDate <= startDate)
                return false;

            if ((endDate - startDate).TotalMinutes % AppointmentDurationInMinutes != 0)
                return false;

            return true;
        }

        private bool IsValidSchedule(IList<dynamic> schedules)
        {
            DateTime currentDateTime = DateTime.Now;
            var processedSchedules = schedules.Select(s => new
            {
                StartDate = new DateTime(s.StartDate.Year, s.StartDate.Month, s.StartDate.Day, s.StartDate.Hour, s.StartDate.Minute, 0),
                EndDate = new DateTime(s.EndDate.Year, s.EndDate.Month, s.EndDate.Day, s.EndDate.Hour, s.EndDate.Minute, 0)
            }).OrderBy(s => s.StartDate).ToList();

            foreach (var schedule in processedSchedules)
            {
                if (schedule.StartDate < currentDateTime)
                    return false;

                if (schedule.EndDate <= schedule.StartDate)
                    return false;

                if ((schedule.EndDate - schedule.StartDate).TotalMinutes % AppointmentDurationInMinutes != 0)
                    return false;
            }

            for (int i = 0; i < processedSchedules.Count - 1; i++)
            {
                if (processedSchedules[i].EndDate > processedSchedules[i + 1].StartDate)
                    return false;
            }

            return true;
        }

        #endregion
    }
}
