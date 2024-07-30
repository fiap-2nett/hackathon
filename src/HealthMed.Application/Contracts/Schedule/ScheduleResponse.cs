using System;

namespace HealthMed.Application.Contracts.Schedule
{
    public sealed class ScheduleResponse
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
