using System;
using System.Threading.Tasks;
using HealthMed.Application.Core.Abstractions.Data;
using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Domain.Repositories;
using HealthMed.Domain.Entities;
using HealthMed.Application.Contracts.Schedule;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Errors;
using HealthMed.Domain.Enumerations;

namespace HealthMed.Application.Services
{
    internal sealed class SchedulesService : IScheduleService
    {
        #region Read-Only Fields

        private readonly IDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUserRepository _userRepository;

        #endregion

        #region Constructors

        public SchedulesService(IDbContext dbContext,
                                IUnitOfWork unitOfWork,
                                IScheduleRepository scheduleService,
                                IUserRepository userRepository)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
            _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(unitOfWork));
            _scheduleRepository = scheduleService ?? throw new ArgumentException(nameof(scheduleService));
            _userRepository = userRepository ?? throw new ArgumentException(nameof(userRepository));

        }

        #endregion

        #region ISchedulesService Members

        public async Task<ScheduleResponse> CreateAsync(int userId, dynamic obj)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            if (user.Name != nameof(UserRoles.Doctor))
                throw new InvalidPermissionException(DomainErrors.Schedule.InvalidPermissions);

            //validar role is doctor
            //validar se as datas estão corretas

            //var schedule = new Schedule(userId, startDate, endDate);  


            //_scheduleRepository.Insert(schedule);
            //await _unitOfWork.SaveChangesAsync();

            return new ScheduleResponse();
        }

        #endregion
    }
}
