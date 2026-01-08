using MediatR;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Application.Services;
using SmartUniversity.Modules.Identity.Domain.Repository;
using SmartUniversity.Modules.Identity.Infrastructure.Persistence;
using SmartUniversity.Modules.Identity.Infrastructure.Security;
using System.Reflection;

namespace SmartUniversity.Modules.Identity
{
    public static class IdentityModule
    {
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddMediatR(typeof(IdentityModule).Assembly);
            
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICookieService, CookieService>();

            services.AddSingleton<IJwtService>(sp =>
            {
                var jwtSecret = configuration["Jwt:Secret"];
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];

                return new JwtService(jwtSecret, issuer, audience);
            });

            return services;
        }
    }
}
