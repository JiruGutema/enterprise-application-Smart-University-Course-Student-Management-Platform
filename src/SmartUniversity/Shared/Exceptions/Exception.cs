namespace SmartUniversity.Shared.Exceptions;

public abstract class DomainExceptionBase : Exception
{
    protected DomainExceptionBase(string message)
        : base(message) { }

    protected DomainExceptionBase(string message, Exception ex)
        : base(message, ex) { }

    public virtual int StatusCode => 400;
}

public abstract class ApplicationExceptionBase : Exception
{
    public virtual int StatusCode => 400;

    protected ApplicationExceptionBase(string message)
        : base(message) { }

    protected ApplicationExceptionBase(string message, Exception innerException)
        : base(message, innerException) { }
}

public abstract class InfrastructureExceptionBase : Exception
{
    public virtual int StatusCode => 400;

    protected InfrastructureExceptionBase(string message)
        : base(message) { }

    protected InfrastructureExceptionBase(string message, Exception ex)
        : base(message, ex) { }
}
