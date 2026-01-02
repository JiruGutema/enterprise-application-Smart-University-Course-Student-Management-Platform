using SmartUniversity.Shared.Exceptions;
namespace SmartUniversity.Modules.Identity.Infrastructure.Exceptions
{
    public abstract class InfrastructureException : InfrastructureExceptionBase
    {
        public virtual int StatusCode => 400;

        protected InfrastructureException(string message)
            : base(message) { }

        protected InfrastructureException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
