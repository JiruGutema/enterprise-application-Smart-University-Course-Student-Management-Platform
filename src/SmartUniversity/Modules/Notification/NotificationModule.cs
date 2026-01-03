using SmartUniversity.Modules.Notification.Application.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Application.Services;
using SmartUniversity.Modules.Notification.Domain.Events;
using SmartUniversity.Modules.Notification.Domain.Repository;
using SmartUniversity.Modules.Notification.Infrastructure;
using SmartUniversity.Modules.Notification.Infrastructure.Persistence;
using SmartUniversity.Shared.Kernel.Interface;

public static class NotificationModule
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services)
    {
        services.AddScoped<INotificationServices, NotificationServices>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<EmailServices>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<UserRegisteredEventHandler>();

        return services;
    }

    public static void SubscribeNotificationEvents(this IApplicationBuilder app)
    {
        var bus = app.ApplicationServices.GetRequiredService<IEventBus>();

        bus.Subscribe<UserRegisteredEvent>(async evt =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<UserRegisteredEventHandler>();
            await handler.HandleAsync(evt);
        });
    }
}
