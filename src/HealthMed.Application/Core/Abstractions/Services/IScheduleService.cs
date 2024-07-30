using HealthMed.Application.Contracts.Schedule;
using System.Threading.Tasks;
using System;

namespace HealthMed.Application.Core.Abstractions.Services
{
    public interface IScheduleService
    {
        Task<ScheduleResponse> CreateAsync(int userId, dynamic obj);
    }
}
