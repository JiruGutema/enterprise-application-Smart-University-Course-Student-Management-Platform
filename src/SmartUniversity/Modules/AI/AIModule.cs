using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartUniversity.Modules.AI.Application.Interfaces;
using SmartUniversity.Modules.AI.Application.Services;
using SmartUniversity.Modules.AI.Domain.Repositories;
using SmartUniversity.Modules.AI.Infrastructure.Persistence;
using SmartUniversity.Modules.AI.Infrastructure.Services;

namespace SmartUniversity.Modules.AI
{
    public static class AIModule
    {
        public static IServiceCollection AddAIModule(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddScoped<IAIRepository, AIRepository>();
            services.AddScoped<IAIService, AIService>();
            services.AddTransient<IOpenAiService, OpenAiService>();

            return services;
        }
    }
}
