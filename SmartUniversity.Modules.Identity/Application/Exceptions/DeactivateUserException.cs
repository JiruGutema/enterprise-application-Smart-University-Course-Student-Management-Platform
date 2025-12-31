
namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public sealed class DeactiveUserException : AppException
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


