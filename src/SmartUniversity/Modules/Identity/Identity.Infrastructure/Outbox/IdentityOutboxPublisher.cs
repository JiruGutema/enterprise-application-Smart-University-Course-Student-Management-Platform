using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Identity.Infrastructure.Persistence;
using SmartUniversity.Shared.Kernel.Interface;

namespace SmartUniversity.Modules.Identity.Infrastructure.Outbox;

public sealed class IdentityOutboxPublisher
{
    private readonly UserDbContext _db;
    private readonly IEventBus _eventBus;

    public IdentityOutboxPublisher(UserDbContext db, IEventBus eventBus)
    {
        _db = db;
        _eventBus = eventBus;
    }

    public async Task PublishPendingAsync(CancellationToken ct = default)
    {
        var messages = await _db.Set<OutboxMessage>()
            .Where(x => x.ProcessedAt == null)
            .OrderBy(x => x.OccurredAt)
            .Take(20)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                var eventMessage = message.Deserialize();
                if (eventMessage is null)
                {
                    message.MarkFailed("Deserialization failed (type unreachable?)");
                }
                else
                {
                    var method = _eventBus.GetType()
                        .GetMethod(nameof(IEventBus.PublishAsync))?
                        .MakeGenericMethod(eventMessage.GetType());

                    if (method is null)
                    {
                        message.MarkFailed("PublishAsync method not found");
                    }
                    else
                    {
                        await (Task)method.Invoke(_eventBus, new[] { eventMessage })!;
                        message.MarkProcessed();
                    }
                }
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
            }
        }

        await _db.SaveChangesAsync(ct);
    }
}
