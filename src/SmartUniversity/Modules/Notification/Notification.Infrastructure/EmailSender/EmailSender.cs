using System.Net;
using System.Net.Mail;
using SmartUniversity.Modules.Notification.Domain.Interfaces;

namespace SmartUniversity.Modules.Notification.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailSender(string host, string port, string user, string password)
        {
            _smtpHost = host;
            _smtpPort = int.Parse(port);
            _smtpUser = user;
            _smtpPass = password;
        }

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

            await client.SendMailAsync(mailMessage, cancellationToken);
        }
    }
}
