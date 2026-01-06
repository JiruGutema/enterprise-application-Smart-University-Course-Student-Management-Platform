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

        Task SendPasswordChangedEmailAsync(
            PasswordChangedEvent evt,
            CancellationToken cancellationToken = default
        );

        Task SendResetPasswordEmailAsync(
            ResetPasswordRequestedEvent evt,
            CancellationToken cancellationToken = default
        );
    }
}
