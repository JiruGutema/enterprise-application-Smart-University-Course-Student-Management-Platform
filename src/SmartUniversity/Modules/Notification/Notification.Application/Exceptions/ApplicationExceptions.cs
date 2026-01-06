using SmartUniversity.Shared.Exceptions;

namespace SmartUniversity.Modules.Notification.Application.Exceptions
{
    public class AppException : ApplicationExceptionBase
    {

      public override int StatusCode => 400;
        public AppException()
            : base("Internal Server Error") { }

        public AppException(string message)
            : base(message) { }

        public AppException(string message, Exception ex)
            : base(message, ex) { }
    }
}
