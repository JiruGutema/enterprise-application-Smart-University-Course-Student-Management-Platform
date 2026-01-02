

using SmartUniversity.Shared.Exceptions;
namespace SmartUniversity.Modules.Identity.Application.Exceptions

{
    public sealed class AppException: ApplicationExceptionBase
    {
        public override int StatusCode => 400;

        public AppException(string message)
            : base(message) { }

        public AppException(string message, Exception innerException)
            : base(message, innerException) { }

        public AppException()
            : base("Error deactivating user.") { }
    }
}
