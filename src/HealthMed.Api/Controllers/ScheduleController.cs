using HealthMed.Api.Contracts;
using HealthMed.Application.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using HealthMed.Application.Contracts.Schedule;
using System.Collections.Generic;
using HealthMed.Application.Core.Abstractions.Authentication;
using HealthMed.Domain.Exceptions;
using HealthMed.Domain.Errors;

namespace HealthMed.Api.Controllers
{
    public class ScheduleController : Controller
    {
        #region Read-Only Fields

        private readonly IScheduleService _scheduleService;
        private readonly IUserSessionProvider _userSessionProvider;

        #endregion

        #region Constructors

        public ScheduleController(IScheduleService scheduleService, IUserSessionProvider userSessionProvider)
        {
            _scheduleService = scheduleService ?? throw new ArgumentException(nameof(scheduleService));
            _userSessionProvider = userSessionProvider;
        }

        #endregion

        #region Endpoints

        [HttpPost(ApiRoutes.Schedule.Create)]
        [ProducesResponseType(typeof(List<ScheduleRequest>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] IList<ScheduleRequest> request)
        {
           if(request is null)
                throw new DomainException(DomainErrors.Schedule.DataSentIsInvalid);

           var idSchedule = await _scheduleService.CreateAsync(_userSessionProvider.IdUser, request);

            return Created(new Uri(Url.ActionLink("teste", "Schedule", new { idSchedule })), idSchedule);
        }

        #endregion
    }
}
