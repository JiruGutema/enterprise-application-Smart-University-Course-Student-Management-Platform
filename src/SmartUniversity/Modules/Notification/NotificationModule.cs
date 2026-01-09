using SmartUniversity.Modules.Identity.Domain.Events;
using SmartUniversity.Modules.Courses.Domain.Events;
using SmartUniversity.Modules.Enrollment.Domain.Events;
using SmartUniversity.Modules.Notification.Application.EventHandlers;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Application.Services;
using SmartUniversity.Modules.Notification.Domain.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Repository;
using SmartUniversity.Modules.Notification.Infrastructure;
using SmartUniversity.Modules.Notification.Infrastructure.Persistence;
using SmartUniversity.Shared.Kernel.Interface;

public static class NotificationModule
{
    public static IServiceCollection AddNotificationModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<INotificationServices, NotificationServices>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IEmailServices, EmailServices>();

        services.AddScoped<IEmailSender>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var password =
                config["SMTP:Password"] ?? throw new ArgumentNullException("SMTP:Password");
            var user = config["SMTP:User"] ?? throw new ArgumentNullException("SMTP:User");
            var host = config["SMTP:Host"] ?? throw new ArgumentNullException("SMTP:Host");
            var port = config["SMTP:Port"] ?? throw new ArgumentNullException("SMTP:Port");
            return new EmailSender(host, port, user, password);
        });
        services.AddScoped<UserRegisteredEventHandler>();
        services.AddScoped<UserLoggedInEventHandler>();
        services.AddScoped<PasswordChangedEventHandler>();
        services.AddScoped<ResetPasswordRequestedEventHandler>();
        
        // Course event handlers
        services.AddScoped<CourseCreatedEventHandler>();
        services.AddScoped<CoursePublishedEventHandler>();
        services.AddScoped<CourseDeletedEventHandler>();
        
        // Enrollment event handlers
        services.AddScoped<StudentEnrolledEventHandler>();
        services.AddScoped<StudentDroppedCourseEventHandler>();
        services.AddScoped<EnrollmentStatusChangedEventHandler>();
        
        // Placeholder handlers for future events
        services.AddScoped<GradingEventHandler>();
        services.AddScoped<AIEventHandler>();

        return services;
    }

    public static void SubscribeNotificationEvents(this IApplicationBuilder app)
    {
        var bus = app.ApplicationServices.GetRequiredService<IEventBus>();
        
        // Subscribe to Identity events
        bus.Subscribe<UserRegisteredEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<UserRegisteredEventHandler>();
            await handler.HandleAsync(evt);
        });

        bus.Subscribe<UserLoggedInEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<UserLoggedInEventHandler>();
            await handler.HandleAsync(evt);
        });

        bus.Subscribe<PasswordChangedEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<PasswordChangedEventHandler>();
            await handler.HandleAsync(evt);
        });

        bus.Subscribe<ResetPasswordRequestedEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ResetPasswordRequestedEventHandler>();
            await handler.HandleAsync(evt);
        });

        // Subscribe to Course events
        bus.Subscribe<CourseCreatedEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<CourseCreatedEventHandler>();
            await handler.HandleAsync(evt);
        });

        bus.Subscribe<CoursePublishedEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<CoursePublishedEventHandler>();
            await handler.HandleAsync(evt);
        });

        bus.Subscribe<CourseDeletedEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<CourseDeletedEventHandler>();
            await handler.HandleAsync(evt);
        });

        // Subscribe to Enrollment events
        bus.Subscribe<StudentEnrolledEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<StudentEnrolledEventHandler>();
            await handler.HandleAsync(evt);
        });

        bus.Subscribe<StudentDroppedCourseEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<StudentDroppedCourseEventHandler>();
            await handler.HandleAsync(evt);
        });

        bus.Subscribe<EnrollmentStatusChangedEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<EnrollmentStatusChangedEventHandler>();
            await handler.HandleAsync(evt);
        });
    }
}
