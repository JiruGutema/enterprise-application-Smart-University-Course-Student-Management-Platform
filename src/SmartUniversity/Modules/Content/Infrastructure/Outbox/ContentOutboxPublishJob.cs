using Microsoft.Extensions.Logging;
using Quartz;

namespace SmartUniversity.Modules.Content.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public class ContentOutboxPublishJob : IJob
{
    private readonly ContentOutboxPublisher _publisher;
    private readonly ILogger<ContentOutboxPublishJob> _logger;

    public ContentOutboxPublishJob(ContentOutboxPublisher publisher, ILogger<ContentOutboxPublishJob> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _publisher.PublishPendingEventsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Content outbox publish job failed");
            // Don't rethrow - let Quartz continue scheduling
        }
    }
}