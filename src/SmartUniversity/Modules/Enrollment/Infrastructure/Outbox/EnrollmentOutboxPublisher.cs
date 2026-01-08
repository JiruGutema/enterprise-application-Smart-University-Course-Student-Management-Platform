using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Enrollment.Domain.Events;
using SmartUniversity.Shared.Kernel.Infrastructure;
using SmartUniversity.Modules.Enrollment.Infrastructure.Persistence;
using SmartUniversity.Shared.Kernel.Interface;

namespace SmartUniversity.Modules.Enrollment.Infrastructure.Outbox
{
    public sealed class EnrollmentOutboxPublisher
    {
        private readonly EnrollmentDbContext _db;
        private readonly IEventBus _eventBus;

        public EnrollmentOutboxPublisher(
            EnrollmentDbContext db,
            IEventBus eventBus)
        {
            _db = db;
            _eventBus = eventBus;
        }

        public async Task PublishAsync(CancellationToken ct = default)
        {
            // Get unprocessed outbox messages, max 20 at a time
            var messages = await _db.Set<OutboxMessage>()
                .Where(x => x.ProcessedOn == null)
                .OrderBy(x => x.OccurredOn)
                .Take(20)
                .ToListAsync(ct);

            foreach (var message in messages)
            {
                try
                {
                    var domainEvent = message.Deserialize();

                    if (domainEvent == null)
                    {
                        message.MarkFailed("Deserialization failed or unknown type");
                        continue;
                    }

                    // Publish the event dynamically
                    var publishMethod = _eventBus.GetType()
                        .GetMethod(nameof(IEventBus.PublishAsync))?
                        .MakeGenericMethod(domainEvent.GetType());

                    if (publishMethod == null)
                    {
                        message.MarkFailed("PublishAsync method not found");
                        continue;
                    }

                    await (Task)publishMethod.Invoke(_eventBus, new[] { domainEvent })!;
                    message.MarkProcessed();
                }
                catch (Exception ex)
                {
                    message.MarkFailed(ex.Message);
                }
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}
