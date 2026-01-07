using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Repositories;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Services;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Outbox;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;

namespace SmartUniversity.Modules.GradingAndAssessment;

public static class GradingAndAssessmentModule
{
    public static IServiceCollection AddGradingAndAssessmentModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(typeof(GradingAndAssessmentModule).Assembly);

        services.AddDbContext<GradingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default"))
                   .AddInterceptors(new GradingOutboxInterceptor()));

        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IGradeRepository, GradeRepository>();
        services.AddScoped<GradeCalculationService>();
        services.AddScoped<GradingOutboxPublisher>();

        return services;
    }
}