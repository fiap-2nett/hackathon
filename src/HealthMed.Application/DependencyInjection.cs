using HealthMed.Application.Core.Abstractions.Services;
using HealthMed.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HealthMed.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
