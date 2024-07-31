using System;

namespace HealthMed.Application.Contracts.Schedule
{
    public sealed class ScheduleRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
