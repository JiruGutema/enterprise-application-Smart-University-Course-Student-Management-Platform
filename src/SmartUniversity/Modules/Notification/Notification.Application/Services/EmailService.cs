using SmartUniversity.Modules.Identity.Domain.Events;
using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Interfaces;

namespace SmartUniversity.Modules.Notification.Application.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IEmailSender _emailSender;

        public EmailServices(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendWelcomeEmailAsync(
            UserRegisteredEvent evt,
            CancellationToken cancellationToken = default
        )
        {
            string subject = "Welcome to Smart University!";
            string body =
                $@"
                <h1>Hello {evt.FullName}!</h1>
                <p>Welcome to Smart University. We're excited to have you on board.</p>
                <p>Login to your account and start exploring your courses.</p>
            ";

            await _emailSender.SendAsync(evt.Email, subject, body, cancellationToken);
        }

        public async Task SendLoginDetectedEmailAsync(
            UserLoggedInEvent evt,
            CancellationToken cancellationToken = default
        )
        {
            string subject = "Welcome to Smart University!";
            string body =
                $@"
                <h1>Hello {evt.FullName}!</h1>
                <p>Login detected from {evt.Location} at {evt.LoginTime:yyyy-MM-dd HH:mm:ss}. If this wasn't you, please secure your account. </p>";

            await _emailSender.SendAsync(evt.Email, subject, body, cancellationToken);
        }

        public async Task SendResetPasswordEmailAsync(
            ResetPasswordRequestedEvent evt,
            CancellationToken cancellationToken = default
        )
        {
            string subject = "Reset password request";
            string body =
                $@"
                <h1>Hello {evt.FullName}!</h1>
                <p>We received a request to reset your password.</p>
                <p>If you made this request, please click the link below to reset your password:</p>
                <p><a href=""{evt.ResetLink}"">Reset Password</a></p>
                <p>If you did not request a password reset, please ignore this email.</p>
                <p>Thank you,<br/>SmartUniversity Team</p>";

            await _emailSender.SendAsync(evt.Email, subject, body, cancellationToken);
        }

        public async Task SendPasswordChangedEmailAsync(
            PasswordChangedEvent evt,
            CancellationToken cancellationToken = default
        )
        {
            string subject = "Your account password has been changed";
            string body =
                $@"
                <h1>Hello {evt.FullName}!</h1>
                <p>Your password has been changed.</p>
                <p>If you made this change, you are not expected to do any thing:</p>
                <p>If you did not change a password, please visit registrar</p>
                <p>Thank you,<br/>SmartUniversity Team</p>";

            await _emailSender.SendAsync(evt.Email, subject, body, cancellationToken);
        }
    }
}
