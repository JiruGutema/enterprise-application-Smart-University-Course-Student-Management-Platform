using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public sealed class UserAlreadyExistsException : ApplicationExceptionBase
    {
        public override int StatusCode => 409;

        public UserAlreadyExistsException(string message)
            : base(message) { }

        public UserAlreadyExistsException()
            : base("A user with the given email already exists.") { }

        public UserAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
