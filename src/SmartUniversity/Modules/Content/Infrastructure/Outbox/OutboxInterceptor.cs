using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartUniversity.Modules.Content.Domain.Events;
using System.Text.Json;

namespace SmartUniversity.Modules.Content.Infrastructure.Outbox;

public class OutboxInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await PublishDomainEventsAsync(eventData.Context);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static async Task PublishDomainEventsAsync(DbContext context)
    {
        var domainEvents = new List<IDomainEvent>();

        // Collect domain events from entities (if implemented)
        // This is a simplified version - in a real implementation,
        // you would collect events from domain entities

        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().Name,
                Content = JsonSerializer.Serialize(domainEvent),
                OccurredOn = domainEvent.OccurredOn
            };

            context.Set<OutboxMessage>().Add(outboxMessage);
        }

        await Task.CompletedTask;
    }
}