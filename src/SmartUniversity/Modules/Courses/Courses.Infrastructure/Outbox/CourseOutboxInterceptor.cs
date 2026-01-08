using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using SmartUniversity.Modules.Courses.Infrastructure.Persistence;
using SmartUniversity.Modules.Courses.Domain.Common;

namespace SmartUniversity.Modules.Courses.Infrastructure.Outbox;

public class CourseOutboxInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not CourseDbContext context)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var domainEvents = context.ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = new OutboxMessage
            {
                Type = domainEvent.GetType().AssemblyQualifiedName!,
                Data = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            };

            context.OutboxMessages.Add(outboxMessage);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}