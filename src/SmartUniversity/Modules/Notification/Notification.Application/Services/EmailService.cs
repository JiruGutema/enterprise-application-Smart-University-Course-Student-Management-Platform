using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Identity.Domain.Events;
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
    }
}
