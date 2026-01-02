using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public sealed class UserRegistrationFailedException : ApplicationExceptionBase
    {
        public override int StatusCode => 400;

        public UserRegistrationFailedException(string message)
            : base(message) { }

        public UserRegistrationFailedException()
            : base("User registration failed.") { }

        public UserRegistrationFailedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
