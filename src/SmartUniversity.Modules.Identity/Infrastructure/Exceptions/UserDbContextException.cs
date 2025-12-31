using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Identity.Infrastructure.Exceptions
{
    public class UserDbContextException : InfrastructureExceptionBase
    {
        public override int StatusCode => 400;

        public UserDbContextException(string message)
            : base(message) { }

        public UserDbContextException(string message, Exception innerException)
            : base(message, innerException) { }

        public UserDbContextException()
            : base("Internal server error") { }
    }
}
