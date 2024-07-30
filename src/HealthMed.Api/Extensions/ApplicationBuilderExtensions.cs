using Microsoft.AspNetCore.Builder;
using HealthMed.Api.Middlewares;

namespace HealthMed.Api.Extensions
{
    internal static class ApplicationBuilderExtensions
    {
        #region Extension Methods

        internal static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder builder)
        {
            builder.UseSwagger();
            builder.UseSwaggerUI(swaggerUiOptions => swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Health&Med API"));

            return builder;
        }

        internal static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionHandlerMiddleware>();

        #endregion
    }
}
