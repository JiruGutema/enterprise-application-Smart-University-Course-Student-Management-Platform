using Quartz;

namespace SmartUniversity.Modules.Enrollment.Infrastructure.Outbox
{
    [DisallowConcurrentExecution]
    public class EnrollmentOutboxPublishJob : IJob
    {
        private readonly EnrollmentOutboxPublisher _publisher;

        public EnrollmentOutboxPublishJob(EnrollmentOutboxPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _publisher.PublishAsync(context.CancellationToken);
        }
    }
}
