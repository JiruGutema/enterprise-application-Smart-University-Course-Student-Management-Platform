
using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Notification.Infrastructure.Exceptions
{
    public sealed class NotificationNotFoundException : InfrastructureExceptionBase
    {
        public override int StatusCode => 404;

        public NotificationNotFoundException(string message)
            : base(message) { }

        public NotificationNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        public NotificationNotFoundException()
            : base("notification not found.") { }
    }
}
