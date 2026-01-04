using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Notification.Infrastructure.Exceptions
{
    public sealed class GetNotificationException : InfrastructureExceptionBase
    {
        public override int StatusCode => 400;

        public GetNotificationException(string message)
            : base(message) { }

        public GetNotificationException(string message, Exception innerException)
            : base(message, innerException) { }

        public GetNotificationException()
            : base("Error retrieving notification.") { }
    }
}
