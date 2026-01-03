using System.Net;
using System.Net.Mail;
using SmartUniversity.Modules.Notification.Application.Interfaces;

namespace SmartUniversity.Modules.Notification.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpHost = "smtp.example.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "your-email@example.com";
        private readonly string _smtpPass = "your-password";

        public async Task SendAsync(
            string to,
            string subject,
            string body,
            CancellationToken cancellationToken = default
        )
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
            };

            var mailMessage = new MailMessage(_smtpUser, to, subject, body) { IsBodyHtml = true };
            Console.WriteLine("\n email is sent with this information, ", mailMessage);

            // await client.SendMailAsync(mailMessage, cancellationToken);
        }
    }
}
