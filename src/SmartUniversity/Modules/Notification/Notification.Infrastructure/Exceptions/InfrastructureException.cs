using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Notification.Infrastructure.Excetpions
{
    public class InfrastructureException : InfrastructureExceptionBase
    {
        protected InfrastructureException(string message)
            : base(message) { }

        protected InfrastructureException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
