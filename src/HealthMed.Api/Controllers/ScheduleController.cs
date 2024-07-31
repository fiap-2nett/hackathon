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
using System.Linq;
using HealthMed.Application.Contracts.Common;

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

        [HttpGet(ApiRoutes.Schedule.GetAll)]
        [ProducesResponseType(typeof(PagedList<ScheduleResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
                => Ok(await _scheduleService.GetAsync(page, pageSize));

        [HttpGet(ApiRoutes.Schedule.GetById)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByid([FromRoute] int idSchedule)
        {
            var response = await _scheduleService.GetByIdAsync(idSchedule);
            if (response is null) return NotFound();

            return Ok(response);
        }


        [HttpPost(ApiRoutes.Schedule.Create)]
        [ProducesResponseType(typeof(List<ScheduleRequest>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] IList<ScheduleRequest> request)
        {
           if(request is null)
                throw new DomainException(DomainErrors.Schedule.DataSentIsInvalid);

            var dynamicList = request.Cast<dynamic>().ToList();
            var createdSchedules = await _scheduleService.CreateAsync(_userSessionProvider.IdUser, dynamicList);

            return Created(Url.Action(nameof(GetAll), "Schedule"), createdSchedules);
        }

        [HttpPut(ApiRoutes.Schedule.Update)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int scheduleId, [FromBody] UpdateScheduleRequest request)
        {
            if (request is null)
                throw new DomainException(DomainErrors.Schedule.NotFound);

            await _scheduleService.Update(_userSessionProvider.IdUser, scheduleId, request.StartDate, request.EndDate);

            return Ok();
        }

        #endregion
    }
}
