namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public sealed class LoginFailedException : AppException
    {
        public override int StatusCode => 409;

        public LoginFailedException()
            : base("Login attempt failed due to invalid credentials.") { }

        public LoginFailedException(string message)
            : base(message) { }

        public LoginFailedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
