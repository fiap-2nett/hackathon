using System;
using HealthMed.Domain.Entities;
using HealthMed.Domain.Extensions;

namespace HealthMed.Application.UnitTests.TestEntities
{
    internal class ScheduleTest : Schedule
    {
        public ScheduleTest(int idSchedule, int idDoctor, DateTime startDate, DateTime endDate)
            : base(idDoctor, startDate.WithoutSeconds(), endDate.WithoutSeconds())
        {
            Id = idSchedule;
        }
    }
}
