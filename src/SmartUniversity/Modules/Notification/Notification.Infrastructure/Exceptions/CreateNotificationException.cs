using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Notification.Infrastructure.Exceptions
{
    public sealed class CreateNotificationException : InfrastructureExceptionBase
    {
        public override int StatusCode => 400;

        public CreateNotificationException(string message)
            : base(message) { }

        public CreateNotificationException(string message, Exception innerException)
            : base(message, innerException) { }

        public CreateNotificationException()
            : base("Error saving notification.") { }
    }
}
