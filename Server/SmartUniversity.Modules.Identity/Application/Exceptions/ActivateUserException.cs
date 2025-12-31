namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public sealed class ActiveUserException : AppException
    {
        public override int StatusCode => 400;

        public ActiveUserException(string message)
            : base(message) { }

        public ActiveUserException(string message, Exception innerException)
            : base(message, innerException) { }

        public ActiveUserException()
            : base("Error deactivating user.") { }
    }
}
