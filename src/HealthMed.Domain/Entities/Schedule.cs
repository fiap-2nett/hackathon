using System;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Domain.Core.Primitives;
using HealthMed.Domain.Core.Utility;

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
    }
}
