using System;
using HealthMed.Application.Contracts.User;

namespace HealthMed.Application.Contracts.Schedule
{
    public sealed class DetailedScheduleResponse
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public UserResponse Doctor { get; set; }
    }
}
