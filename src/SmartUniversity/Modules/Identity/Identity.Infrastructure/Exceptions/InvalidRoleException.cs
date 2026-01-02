using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Identity.Infrastructure.Exceptions
{
    public sealed class InvalidRoleException : InfrastructureExceptionBase
    {
        public override int StatusCode => 400;

        public InvalidRoleException(string message)
            : base(message) { }

        public InvalidRoleException(string message, Exception innerException)
            : base(message, innerException) { }

        public InvalidRoleException()
            : base("Invalid role specified.") { }
    }
}
