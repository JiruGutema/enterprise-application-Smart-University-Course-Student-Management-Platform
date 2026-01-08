using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.Courses.Infrastructure.Persistence;
using SmartUniversity.Shared.Kernel.Interface;

namespace SmartUniversity.Modules.Courses.Infrastructure.Outbox;

public class CourseOutboxPublisher
{
    private readonly CourseDbContext _context;
    private readonly IEventBus _eventBus;

    public CourseOutboxPublisher(CourseDbContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task PublishPendingAsync(CancellationToken ct = default)
    {
        var messages = await _context.OutboxMessages
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
                    message.MarkFailed("Deserialization failed");
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

        await _context.SaveChangesAsync(ct);
    }
}