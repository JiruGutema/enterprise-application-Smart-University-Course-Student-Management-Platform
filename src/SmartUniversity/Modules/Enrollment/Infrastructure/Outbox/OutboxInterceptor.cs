using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartUniversity.Modules.Enrollment.Domain.Aggregates;

namespace SmartUniversity.Modules.Enrollment.Infrastructure.Outbox
{
    public sealed class OutboxInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null)
                return new ValueTask<InterceptionResult<int>>(result);

            // Materialize the entries first to avoid modifying the live collection during iteration
            var entries = context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment)
                .ToList();

            if (!entries.Any())
                return new ValueTask<InterceptionResult<int>>(result);

            var outboxMessages = new List<OutboxMessage>();

            foreach (var entry in entries)
            {
                if (entry.Entity is SmartUniversity.Modules.Enrollment.Domain.Aggregates.Enrollment enrollment)
                {
                    var domainEvents = enrollment.DomainEvents;
                    if (domainEvents == null || domainEvents.Count == 0)
                        continue;

                    foreach (var domainEvent in domainEvents)
                    {
                        // Create OutboxMessage objects and collect them (do NOT add to context yet)
                        outboxMessages.Add(OutboxMessage.From(domainEvent));
                    }

                    // Clear domain events to avoid duplicate publishing
                    enrollment.ClearDomainEvents();
                }
            }

            if (outboxMessages.Count > 0)
            {
                // Add all outbox messages in one go — this will register them in the ChangeTracker
                context.Set<OutboxMessage>().AddRange(outboxMessages);
            }

            return new ValueTask<InterceptionResult<int>>(result);
        }
    }
}
