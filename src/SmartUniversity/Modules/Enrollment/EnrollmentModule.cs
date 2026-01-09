using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

using SmartUniversity.Modules.Enrollment.Application;
using SmartUniversity.Modules.Enrollment.Application.EventHandlers;
using SmartUniversity.Modules.Enrollment.Domain.Repositories;
using SmartUniversity.Modules.Enrollment.Infrastructure.Persistence;
using SmartUniversity.Modules.Enrollment.Infrastructure.Repositories;

using SmartUniversity.Shared.Kernel.Interface;
using SmartUniversity.Modules.Courses.Domain.Events;
using SmartUniversity.Modules.Identity.Domain.Events;

namespace SmartUniversity.Modules.Enrollment;

public static class EnrollmentModule
{

    public static IServiceCollection AddEnrollmentModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<CourseEventHandler>();
        services.AddScoped<IdentityEventHandler>();

        return services;
    }

  
    public static void SubscribeEnrollmentEvents(this IApplicationBuilder app)
    {
        var bus = app.ApplicationServices.GetRequiredService<IEventBus>();

    bus.Subscribe<CourseDeletedEvent>(async evt =>
{
    using var scope = app.ApplicationServices.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<CourseEventHandler>();
    await handler.HandleCourseDeletedAsync(evt);
});

bus.Subscribe<UserAccountDeactivatedEvent>(async evt =>
{
    using var scope = app.ApplicationServices.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<IdentityEventHandler>();
    await handler.HandleUserAccountDeactivatedAsync(evt);
});

    }
}
