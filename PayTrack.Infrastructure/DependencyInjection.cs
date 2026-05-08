using Microsoft.Extensions.DependencyInjection;
using PayTrack.Application.Interfaces;
using PayTrack.Infrastructure.Services;

namespace PayTrack.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}
