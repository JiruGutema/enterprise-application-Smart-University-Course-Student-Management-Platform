using SmartUniversity.Modules.Identity.Domain.Events;

namespace SmartUniversity.Modules.Notification.Application.Interfaces
{
    public interface IEmailServices
    {
        Task SendWelcomeEmailAsync(
            UserRegisteredEvent evt,
            CancellationToken cancellationToken = default
        );

        Task SendLoginDetectedEmailAsync(
            UserLoggedInEvent evt,
            CancellationToken cancellationToken = default
        );
    }
}
