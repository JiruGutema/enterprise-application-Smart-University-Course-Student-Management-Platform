
namespace SmartUniversity.Modules.Identity.Infrastructure.Exceptions
{
    public sealed class DeactiveUserException : InfrastructureException
    {
        public override int StatusCode => 400;

        public DeactiveUserException(string message)
            : base(message) { }

        public DeactiveUserException(string message, Exception innerException)
            : base(message, innerException) { }

        public DeactiveUserException()
            : base("Error deactivating user.") { }
    }
}

