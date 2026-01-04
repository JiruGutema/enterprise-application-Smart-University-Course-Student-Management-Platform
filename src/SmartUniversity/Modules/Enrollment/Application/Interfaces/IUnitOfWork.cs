namespace SmartUniversity.Modules.Enrollment.Application;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct);
}
