namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public class AppException : Exception
    {
        public virtual int StatusCode => 400;

        public AppException(string message)
            : base(message) { }

        public AppException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
