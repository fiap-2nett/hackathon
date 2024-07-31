using System;
using System.Threading.Tasks;
using HealthMed.Api.Contracts;
using HealthMed.Api.Infrastructure;
using HealthMed.Application.Contracts.Appointment;
using HealthMed.Application.Contracts.Common;
using HealthMed.Application.Core.Abstractions.Authentication;
using HealthMed.Application.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthMed.Api.Controllers
{    
    public sealed class AppointmentController : ApiController
    {
        #region Read-Only Fields

        private readonly IAppointmentService _appointmentService;
        private readonly IUserSessionProvider _userSessionProvider;

        #endregion

        #region Constructors

        public AppointmentController(IAppointmentService appointmentService, IUserSessionProvider userSessionProvider)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _userSessionProvider = userSessionProvider ?? throw new ArgumentNullException(nameof(userSessionProvider));
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Represents the query to retrieve all appointments.
        /// </summary>
        /// <param name="idUserDoctor">The doctor identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size. The max page size is 100.</param>
        /// <param name="fromDate">The page size. The max page size is 100.</param>
        /// <returns>The paged list of the appointments.</returns>
        [HttpGet(ApiRoutes.Appointment.List)]
        [ProducesResponseType(typeof(PagedList<AppointmentResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromRoute] int idUserDoctor,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? fromDate = null)
        {
            return Ok(await _appointmentService.ListAsync(new ListAppointmentRequest(page, pageSize), idUserDoctor, fromDate));
        }

        /// <summary>
        /// Represents the request to reserve appointment to the logged in user.
        /// </summary>
        /// <param name="idUserDoctor">The doctor identifier.</param>
        /// <param name="idAppointment">The appointment identifier.</param>
        [HttpPost(ApiRoutes.Appointment.Reserve)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reserve([FromRoute] int idUserDoctor, [FromRoute] int idAppointment)
        {
            await _appointmentService.ReserveAsync(idAppointment, idUserDoctor, idUserPatient: _userSessionProvider.IdUser);
            return NoContent();
        }

        #endregion
    }
}
