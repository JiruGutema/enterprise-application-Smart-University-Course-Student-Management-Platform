using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public sealed class UserNotFoundException : ApplicationExceptionBase
    {
        public override int StatusCode => 404;

        public UserNotFoundException()
            : base("User Not Found") { }

        public UserNotFoundException(string message)
            : base(message) { }

        public UserNotFoundException(string message, Exception exception)
            : base(message, exception) { }
    }
}
