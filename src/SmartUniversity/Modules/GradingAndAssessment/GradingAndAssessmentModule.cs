using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SmartUniversity.Modules.Courses.Domain.Events;
using SmartUniversity.Modules.Enrollment.Domain.Events;
using SmartUniversity.Modules.GradingAndAssessment.Application.EventHandlers;
using SmartUniversity.Modules.GradingAndAssessment.Application.Services;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Repositories;
using SmartUniversity.Modules.GradingAndAssessment.Domain.Services;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Outbox;
using SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Persistence;
using SmartUniversity.Modules.Identity.Domain.Events;
using SmartUniversity.Shared.Kernel.Interface;

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
        
        // Add lookup service for cross-module data
        services.AddScoped<IEnrollmentLookupService, EnrollmentLookupService>();
        
        // Add event handlers for cross-module events
        services.AddScoped<EnrollmentEventHandler>();
        services.AddScoped<CourseEventHandler>();
        services.AddScoped<IdentityEventHandler>();

        return services;
    }

    public static void SubscribeGradingEvents(this IApplicationBuilder app)
    {
        var bus = app.ApplicationServices.GetRequiredService<IEventBus>();

        // Subscribe to enrollment events
        bus.Subscribe<StudentEnrolledEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<EnrollmentEventHandler>();
            await handler.HandleStudentEnrolledAsync(evt);
        });

        bus.Subscribe<StudentDroppedCourseEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<EnrollmentEventHandler>();
            await handler.HandleStudentDroppedCourseAsync(evt);
        });

        bus.Subscribe<EnrollmentStatusChangedEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<EnrollmentEventHandler>();
            await handler.HandleEnrollmentStatusChangedAsync(evt);
        });

        // Subscribe to course events
        bus.Subscribe<CourseCreatedEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<CourseEventHandler>();
            await handler.HandleCourseCreatedAsync(evt);
        });

        // Subscribe to identity events
        bus.Subscribe<UserRegisteredEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IdentityEventHandler>();
            await handler.HandleUserRegisteredAsync(evt);
        });
    }
}