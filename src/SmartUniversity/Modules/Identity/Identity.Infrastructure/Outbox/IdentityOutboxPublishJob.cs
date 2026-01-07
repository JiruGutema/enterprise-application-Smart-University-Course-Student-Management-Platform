using Quartz;

namespace SmartUniversity.Modules.Identity.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class IdentityOutboxPublishJob : IJob
{
    private readonly IdentityOutboxPublisher _publisher;

    public IdentityOutboxPublishJob(IdentityOutboxPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _publisher.PublishPendingAsync(context.CancellationToken);
    }
}
