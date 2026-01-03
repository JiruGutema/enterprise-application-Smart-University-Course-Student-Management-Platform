using SmartUniversity.Modules.Notification.Application.Interfaces;
using SmartUniversity.Modules.Notification.Domain.Events;

namespace SmartUniversity.Modules.Notification.Application.Services
{
    public class EmailServices
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
    }
}
