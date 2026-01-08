using Quartz;

namespace SmartUniversity.Modules.Courses.Infrastructure.Outbox;

public class CourseOutboxPublishJob : IJob
{
    private readonly CourseOutboxPublisher _publisher;

    public CourseOutboxPublishJob(CourseOutboxPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _publisher.PublishPendingAsync(context.CancellationToken);
    }
}