namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public sealed class InvalidUserException : AppException
    {
        public override int StatusCode => 409;

        public InvalidUserException()
            : base("Invalid user information.") { }

        public InvalidUserException(string message)
            : base(message) { }

        public InvalidUserException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
