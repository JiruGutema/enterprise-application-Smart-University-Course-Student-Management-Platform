namespace SmartUniversity.Modules.Identity.Infrastructure.Exceptions
{
    public sealed class PasswordHashException : InfrastructureException
    {
        public override int StatusCode => 400;

        public PasswordHashException(string message)
            : base(message) { }

        public PasswordHashException(string message, Exception innerException)
            : base(message, innerException) { }

        public PasswordHashException()
            : base("Error deactivating user.") { }
    }
}
