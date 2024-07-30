using System;
using System.Collections.Generic;
using System.Linq;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Domain.Core.Primitives;
using HealthMed.Domain.Core.Utility;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Exceptions;

namespace HealthMed.Domain.Entities
{
    public class Schedule : AggregateRoot<int>, IAuditableEntity
    {
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

            Ensure.GreaterThan(startDate, DateTime.MinValue, $"The StartDate must be greater than {DateTime.MinValue:dd/MM/yyyy}.", nameof(startDate));
            Ensure.GreaterThan(endDate, startDate, $"The EndDate must be greater than {startDate:dd/MM/yyyy}.", nameof(endDate));

            StartDate = startDate;
            EndDate = endDate;
        }

        public static IReadOnlyCollection<Schedule> CreateSchedules(int idDoctor, IList<dynamic> schedules)
        {
            if (schedules == null || !schedules.Any())
                throw new ArgumentException("The schedules list cannot be null or empty.", nameof(schedules));

            var scheduleList = new List<Schedule>();

            foreach (var schedule in schedules)
            {
                DateTime startDate = new DateTime(
                    schedule.StartDate.Year,
                    schedule.StartDate.Month,
                    schedule.StartDate.Day,
                    schedule.StartDate.Hour,
                    schedule.StartDate.Minute,
                    0);

                DateTime endDate = new DateTime(
                    schedule.EndDate.Year,
                    schedule.EndDate.Month,
                    schedule.EndDate.Day,
                    schedule.EndDate.Hour,
                    schedule.EndDate.Minute,
                    0);

                var newSchedule = new Schedule(idDoctor, startDate, endDate);
                scheduleList.Add(newSchedule);
            }

            return scheduleList;
        }

        #endregion
    }
}
