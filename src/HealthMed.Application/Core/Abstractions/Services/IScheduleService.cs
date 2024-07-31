using HealthMed.Application.Contracts.Schedule;
using System.Threading.Tasks;
using System.Collections.Generic;
using HealthMed.Application.Contracts.Common;
using System;

namespace HealthMed.Application.Core.Abstractions.Services
{
    public interface IScheduleService
    {
        Task<PagedList<ScheduleResponse>> GetAsync(int page, int pageSize);
        Task<DetailedScheduleResponse> GetByIdAsync(int idSchedule);
        Task<List<ScheduleResponse>> CreateAsync(int idUserPerformedAction, List<(DateTime StartDate, DateTime EndDate)> periods);
        Task<ScheduleResponse> Update(int idUserPerformedAction, int idSchedule, DateTime startDate, DateTime endDate);
    }
}
