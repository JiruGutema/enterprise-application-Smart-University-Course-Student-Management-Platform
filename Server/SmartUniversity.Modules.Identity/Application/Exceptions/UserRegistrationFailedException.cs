namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public sealed class UserRegistrationFailedException : AppException
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
