using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HealthMed.Application.Core.Abstractions.Authentication;
using HealthMed.Application.Core.Abstractions.Cryptography;
using HealthMed.Domain.Core.Abstractions;
using HealthMed.Infrastructure.Authentication;
using HealthMed.Infrastructure.Authentication.Settings;
using HealthMed.Infrastructure.Cryptography;

namespace HealthMed.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]))
                    };
                });

            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SettingsKey));

            services.AddScoped<IUserSessionProvider, UserSessionProvider>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IPasswordHashChecker, PasswordHasher>();

            return services;
        }
    }
}
