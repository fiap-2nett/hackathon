using System;
using System.Collections.Generic;
using System.Linq;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Domain.Core.Primitives;
using HealthMed.Domain.Core.Utility;
using HealthMed.Domain.Enumerations;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Extensions;

namespace HealthMed.Domain.Entities
{
    public class Schedule : AggregateRoot<int>, IAuditableEntity
    {
        #region Constants

        private const int AppointmentDurationDefaultInMinutes = 60;

        #endregion

        #region Properties

        public int IdDoctor { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        #endregion

        #region Constructors

        private Schedule()
        { }

        public Schedule(int idDoctor, DateTime startDate, DateTime endDate)
        {
            Ensure.GreaterThan(idDoctor, 0, "The IdDoctor must be greater than zero.", nameof(idDoctor));
            Ensure.GreaterThan(startDate, DateTime.MinValue, $"The StartDate must be greater than {DateTime.MinValue:dd/MM/yyyy}.", nameof(startDate));
            Ensure.GreaterThan(endDate, startDate, $"The EndDate must be greater than {startDate:dd/MM/yyyy}.", nameof(endDate));

            if (startDate == endDate)
                throw new DomainException(DomainErrors.Schedule.DifferentDate);

            IdDoctor = idDoctor;
            StartDate = startDate;
            EndDate = endDate;
        }

        #endregion

        #region Public Methods

        public void Update(DateTime startDate, DateTime endDate, User userPerformedAction)
        {
            if (userPerformedAction.Id != IdDoctor)
                throw new InvalidPermissionException(DomainErrors.Schedule.InvalidPermissions);

            if (startDate == endDate)
                throw new DomainException(DomainErrors.Schedule.DifferentDate);

            Ensure.GreaterThan(startDate, DateTime.MinValue, $"The StartDate must be greater than {DateTime.MinValue:dd/MM/yyyy}.", nameof(startDate));
            Ensure.GreaterThan(endDate, startDate, $"The EndDate must be greater than {startDate:dd/MM/yyyy}.", nameof(endDate));

            StartDate = startDate;
            EndDate = endDate;
        }

        #endregion

        #region Static Methods

        public static bool IsValidSchedule(DateTime startDate, DateTime endDate, int? appointmentDurationInMinutes = null)
        {
            appointmentDurationInMinutes ??= AppointmentDurationDefaultInMinutes;

            if (startDate.WithoutSeconds() < DateTime.Now.WithoutSeconds())
                return false;

            if (endDate.WithoutSeconds() <= startDate.WithoutSeconds())
                return false;

            if ((endDate.WithoutSeconds() - startDate.WithoutSeconds()).TotalMinutes % appointmentDurationInMinutes != 0)
                return false;

            return true;
        }

        public static bool IsValidSchedule(List<(DateTime StartDate, DateTime EndDate)> intervals, int? appointmentDurationInMinutes = null)
        {
            appointmentDurationInMinutes ??= AppointmentDurationDefaultInMinutes;

            var processedSchedules = intervals.Select(s => new { StartDate = s.StartDate.WithoutSeconds(), EndDate = s.EndDate.WithoutSeconds() })
                .OrderBy(s => s.StartDate)
                .ToList();

            foreach (var schedule in processedSchedules)
            {
                if (schedule.StartDate < DateTime.Now.WithoutSeconds())
                    return false;

                if (schedule.EndDate <= schedule.StartDate)
                    return false;

                if ((schedule.EndDate - schedule.StartDate).TotalMinutes % appointmentDurationInMinutes != 0)
                    return false;
            }

            for (int i = 0; i < processedSchedules.Count - 1; i++)
            {
                if (processedSchedules[i].EndDate > processedSchedules[i + 1].StartDate)
                    return false;
            }

            return true;
        }

        public static IReadOnlyCollection<Schedule> CreateSchedules(User userDoctor, List<(DateTime StartDate, DateTime EndDate)> intervals)
        {
            if (userDoctor.IdRole != (int)UserRoles.Doctor)
                throw new InvalidPermissionException(DomainErrors.Schedule.InvalidPermissions);

            if (intervals.IsNullOrEmpty())
                throw new ArgumentException("The intervals list cannot be null or empty.", nameof(intervals));

            return intervals.Select(schedule => new Schedule(userDoctor.Id, schedule.StartDate.WithoutSeconds(), schedule.EndDate.WithoutSeconds()))
                .ToList();
        }

        #endregion
    }
}
