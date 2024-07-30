using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthMed.Api.Constants;
using HealthMed.Api.Contracts;
using HealthMed.Domain.Core.Primitives;

namespace HealthMed.Api.Infrastructure
{
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ApiController : ControllerBase
    {
        #region Methods

        protected new IActionResult Ok(object value)
            => base.Ok(value);

        protected IActionResult BadRequest(Error error)
            => BadRequest(new ApiErrorResponse(error));

        protected new IActionResult NotFound()
            => NotFound(Errors.NotFoudError.Message);

        #endregion
    }
}
