using Quartz;

namespace SmartUniversity.Modules.GradingAndAssessment.Infrastructure.Outbox;

public class GradingOutboxPublishJob : IJob
{
    private readonly GradingOutboxPublisher _publisher;

    public GradingOutboxPublishJob(GradingOutboxPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _publisher.PublishPendingAsync(context.CancellationToken);
    }
}