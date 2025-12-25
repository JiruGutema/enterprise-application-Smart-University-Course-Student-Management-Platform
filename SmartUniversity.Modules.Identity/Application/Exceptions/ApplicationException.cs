namespace SmartUniversity.Modules.Identity.Application.Exceptions
{
    public abstract class AppException : Exception
    {
        public virtual int StatusCode => 400;

        protected AppException(string message)
            : base(message) { }

        protected AppException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
