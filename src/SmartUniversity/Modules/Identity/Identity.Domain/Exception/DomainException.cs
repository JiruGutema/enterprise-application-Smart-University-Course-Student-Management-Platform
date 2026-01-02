using SmartUniversity.Shared.Exceptions;
namespace SmartUniversity.Modules.Identity.Domain.Exceptions
{
    public abstract class DomainException : DomainExceptionBase
    {
        public virtual int StatusCode => 400;

        protected DomainException(string message)
            : base(message) { }

        protected DomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
