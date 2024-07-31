using System;
using HealthMed.Domain.Entities;

namespace HealthMed.Application.UnitTests.TestEntities
{
    internal class ScheduleTest : Schedule
    {
        public ScheduleTest(int idSchedule, int idDoctor, DateTime startDate, DateTime endDate)
            : base(idDoctor, startDate, endDate)
        {
            Id = idSchedule;
        }
    }
}
