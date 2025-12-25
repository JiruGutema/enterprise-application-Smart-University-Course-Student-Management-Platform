namespace SmartUniversity.Modules.Identity.Infrastructure.Exceptions
{
    public abstract class InfrastructureException : Exception
    {
        public virtual int StatusCode => 400;

        protected InfrastructureException(string message)
            : base(message) { }

        protected InfrastructureException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
