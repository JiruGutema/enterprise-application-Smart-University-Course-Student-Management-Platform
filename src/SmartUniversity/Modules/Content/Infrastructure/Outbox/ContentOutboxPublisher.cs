using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartUniversity.Modules.Content.Infrastructure.Persistence;
using SmartUniversity.Shared.Kernel.Interface;
using System.Text.Json;

namespace SmartUniversity.Modules.Content.Infrastructure.Outbox;

public class ContentOutboxPublisher
{
    private readonly ContentDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ContentOutboxPublisher> _logger;

    public ContentOutboxPublisher(ContentDbContext context, IEventBus eventBus, ILogger<ContentOutboxPublisher> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task PublishPendingEventsAsync()
    {
        try
        {
            var pendingMessages = await _context.OutboxMessages
                .Where(m => m.ProcessedOn == null)
                .OrderBy(m => m.OccurredOn)
                .Take(100) // Process in batches
                .ToListAsync();

            foreach (var message in pendingMessages)
            {
                try
                {
                    // Deserialize and publish the event
                    var eventType = Type.GetType(message.Type);
                    if (eventType != null)
                    {
                        var eventData = JsonSerializer.Deserialize(message.Content, eventType);
                        if (eventData != null)
                        {
                            await _eventBus.PublishAsync(eventData);
                        }
                    }

                    // Mark as processed
                    message.ProcessedOn = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other messages
                    _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                    message.Error = ex.Message;
                }
            }

            if (pendingMessages.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log database connection or other infrastructure errors
            _logger.LogError(ex, "Failed to retrieve or process outbox messages");
        }
    }
}
