namespace SmartUniversity.Modules.Identity.Infrastructure.Exceptions
{
    public sealed class RepositoryException : InfrastructureException
    {
        public override int StatusCode => 400;

        public RepositoryException(string message)
            : base(message) { }

        public RepositoryException(string message, Exception innerException)
            : base(message, innerException) { }

        public RepositoryException()
            : base("Error performing repository operation") { }
    }
}
