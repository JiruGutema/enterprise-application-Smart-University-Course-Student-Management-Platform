# Transactional Outbox Pattern Implementation in SmartUniversity

## Executive Summary

The SmartUniversity system implements the **Transactional Outbox Pattern** with sophisticated retry mechanisms to ensure reliable event publishing in distributed microservices. Events are stored atomically with business data and processed with exponential backoff retry strategy.

**Key Features**: Atomic storage • Exponential backoff (2min→32min) • Dead letter handling • Background processing every 10s

## Implementation Flow Diagram

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   User Action   │───▶│  Domain Entity   │───▶│  Domain Event   │
│ (Register User) │    │ (User.Register)  │    │ (UserRegistered)│
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                                         │
                                                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Database Transaction                         │
│  ┌─────────────────┐              ┌─────────────────────────┐   │
│  │   Business Data │              │    Outbox Message       │   │
│  │   (User Table)  │◀────────────▶│  (Event + Metadata)     │   │
│  └─────────────────┘              └─────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                                ▼ COMMIT (Atomic)
┌─────────────────────────────────────────────────────────────────┐
│                    Background Job (Every 10s)                  │
│                                                                 │
│  ┌─────────────────┐    ┌──────────────┐    ┌─────────────────┐│
│  │ Fetch Pending   │───▶│ Retry Logic  │───▶│ Publish Event   ││
│  │   Messages      │    │ & Backoff    │    │  to RabbitMQ    ││
│  └─────────────────┘    └──────────────┘    └─────────────────┘│
│                                │                               │
│                                ▼                               │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │              Retry Strategy                                 ││
│  │  Retry 1: 2min  │ Retry 2: 4min  │ ... │ Dead Letter      ││
│  │  Retry 3: 8min  │ Retry 4: 16min │ ... │ (After 5 fails) ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Event Consumers                           │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │  Notification   │  │   Enrollment    │  │     Other       │ │
│  │    Service      │  │    Service      │  │   Services      │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

## Overview

The SmartUniversity system implements the **Transactional Outbox Pattern** to ensure reliable event publishing in a distributed microservices architecture. This pattern guarantees that domain events are published exactly once, even in the face of system failures, by storing events in the same database transaction as the business data.

## Architecture Components

### 1. Core Kernel Infrastructure

#### AggregateRoot Base Class
```csharp
public abstract class AggregateRoot
{
    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(object domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

**Purpose**: Provides a base class for domain entities that can raise domain events. Events are collected in memory until they're processed by the outbox interceptor.

#### Event Bus Interface
```csharp
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
}
```

**Purpose**: Abstracts the messaging infrastructure (RabbitMQ in this case) for publishing and subscribing to events.

### 2. Outbox Message Entity

```csharp
public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? Error { get; private set; }

    public static OutboxMessage FromEvent(object @event)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = @event.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(@event),
            OccurredAt = DateTime.UtcNow,
        };
    }
}
```

**Key Features**:
- **Type**: Stores the full assembly-qualified name for proper deserialization
- **Payload**: JSON serialized event data stored as JSONB in PostgreSQL
- **ProcessedAt**: Tracks successful processing (null = pending)
- **RetryCount**: Enables retry logic with exponential backoff
- **Error**: Stores failure reasons for debugging

### 3. Database Schema

The outbox table is created in the Identity schema:

```sql
CREATE TABLE identity.outbox_messages (
    Id uuid PRIMARY KEY,
    Type text NOT NULL,
    Payload jsonb NOT NULL,
    OccurredAt timestamp with time zone NOT NULL,
    ProcessedAt timestamp with time zone NULL,
    RetryCount integer NOT NULL,
    Error text NULL
);
```

**Design Decisions**:
- **JSONB**: Efficient storage and querying of event payloads
- **UUID**: Globally unique identifiers for distributed systems
- **Nullable ProcessedAt**: Simple pending/processed state tracking

## Step-by-Step Implementation

### Step 1: Domain Event Generation

When business operations occur, domain entities raise events:

```csharp
public class User : AggregateRoot
{
    public User(Guid id, string email, string fullName, Role role, string passwordHash)
    {
        // ... validation and assignment
        
        // Raise domain event
        AddDomainEvent(new UserRegisteredEvent(Id, Email, FullName));
    }

    public void UpdateEmail(string email)
    {
        Email = email;
        AddDomainEvent(new UserEmailUpdatedEvent(Id, Email, FullName));
    }
}
```

**Process**:
1. Business logic executes
2. Domain events are added to the aggregate's event collection
3. Events remain in memory until database save

### Step 2: Transactional Outbox Interceptor

The interceptor captures domain events during database saves:

```csharp
public sealed class IdentityOutboxInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;
        if (dbContext is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        // Extract domain events from tracked aggregates
        var outboxMessages = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
            {
                var domainEvents = aggregate.DomainEvents.ToList();
                aggregate.ClearDomainEvents(); // Clear after extraction
                return domainEvents;
            })
            .Select(domainEvent => OutboxMessage.FromEvent(domainEvent))
            .ToList();

        // Add outbox messages to the same transaction
        if (outboxMessages.Count > 0)
        {
            dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

**Critical Points**:
1. **Same Transaction**: Outbox messages are saved in the same database transaction as business data
2. **Event Extraction**: Domain events are extracted from all tracked aggregates
3. **Event Clearing**: Events are cleared from aggregates after extraction
4. **Atomic Operation**: Either both business data and events are saved, or neither

### Step 3: Background Event Publishing

The outbox publisher processes pending events:

```csharp
public sealed class IdentityOutboxPublisher
{
    public async Task PublishPendingAsync(CancellationToken ct = default)
    {
        // Fetch pending messages in chronological order
        var messages = await _db.Set<OutboxMessage>()
            .Where(x => x.ProcessedAt == null)
            .OrderBy(x => x.OccurredAt)
            .Take(20) // Batch processing
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                // Deserialize event
                var eventMessage = message.Deserialize();
                if (eventMessage is null)
                {
                    message.MarkFailed("Deserialization failed");
                    continue;
                }

                // Publish via event bus using reflection
                var method = _eventBus.GetType()
                    .GetMethod(nameof(IEventBus.PublishAsync))?
                    .MakeGenericMethod(eventMessage.GetType());

                await (Task)method.Invoke(_eventBus, new[] { eventMessage })!;
                message.MarkProcessed(); // Mark as successfully processed
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message); // Track failure for retry
            }
        }

        await _db.SaveChangesAsync(ct); // Persist processing status
    }
}
```

**Key Features**:
- **Batch Processing**: Processes up to 20 messages per run for efficiency
- **Chronological Order**: Ensures events are published in the correct sequence
- **Error Handling**: Failed events are marked with error details for debugging
- **Idempotency**: Successfully processed events are marked to prevent reprocessing

### Step 4: Scheduled Job Execution

Quartz.NET scheduler runs the publisher periodically:

```csharp
[DisallowConcurrentExecution]
public class IdentityOutboxPublishJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await _publisher.PublishPendingAsync(context.CancellationToken);
    }
}
```

**Configuration in Program.cs**:
```csharp
builder.Services.AddQuartz(q =>
{
    var identityJobKey = new JobKey("IdentityOutboxPublishJob");
    q.AddJob<IdentityOutboxPublishJob>(opts => opts.WithIdentity(identityJobKey));
    q.AddTrigger(opts => opts
        .ForJob(identityJobKey)
        .WithIdentity("IdentityOutboxPublishJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(10) // Run every 10 seconds
            .RepeatForever()));
});
```

**Scheduling Features**:
- **DisallowConcurrentExecution**: Prevents overlapping job executions
- **10-second interval**: Balances latency with system load
- **Automatic retry**: Failed jobs are retried on the next schedule

## Retry Strategy and Error Handling

### Enhanced Retry Mechanism

The system implements sophisticated retry logic with exponential backoff and dead letter handling:

```csharp
public void MarkFailed(string error)
{
    RetryCount++;
    Error = error;
}

public bool ShouldRetry()
{
    return RetryCount < 5 && 
           DateTime.UtcNow > OccurredAt.AddMinutes(Math.Pow(2, RetryCount));
}

public bool HasExceededMaxRetries()
{
    return RetryCount >= 5;
}

public void MarkAsDeadLetter(string reason)
{
    Error = $"Dead Letter: {reason}";
}
```

**Retry Strategy Features**:
- **Maximum 5 retry attempts** to prevent infinite loops
- **Exponential backoff** with increasing delays between retries
- **Dead letter handling** for permanently failed messages
- **Comprehensive error tracking** for debugging and monitoring

### Exponential Backoff Schedule

| Retry Attempt | Delay Before Retry | Total Time Since First Failure |
|---------------|-------------------|--------------------------------|
| 1st retry     | 2 minutes         | 2 minutes                      |
| 2nd retry     | 4 minutes         | 6 minutes                      |
| 3rd retry     | 8 minutes         | 14 minutes                     |
| 4th retry     | 16 minutes        | 30 minutes                     |
| 5th retry     | 32 minutes        | 62 minutes                     |
| Dead Letter   | No more retries   | After 5 failed attempts       |

### Enhanced Publisher Logic

```csharp
public async Task PublishPendingAsync(CancellationToken ct = default)
{
    var messages = await _db.Set<OutboxMessage>()
        .Where(x => x.ProcessedAt == null)
        .OrderBy(x => x.OccurredAt)
        .Take(20)
        .ToListAsync(ct);

    foreach (var message in messages)
    {
        // Handle messages that have exceeded max retries
        if (message.HasExceededMaxRetries())
        {
            message.MarkAsDeadLetter("Exceeded maximum retry attempts");
            continue;
        }

        // Skip messages that shouldn't be retried yet (exponential backoff)
        if (!message.ShouldRetry())
            continue;

        // ... rest of publishing logic
    }
}
```

### Error Scenarios Handled

1. **Deserialization Failures**: When event types can't be resolved
2. **Publishing Failures**: When RabbitMQ is unavailable
3. **Network Issues**: Temporary connectivity problems
4. **Serialization Issues**: Malformed event data
5. **Transient Service Outages**: Temporary downstream service failures
6. **Resource Exhaustion**: Memory or connection pool issues

## Message Flow Diagram

```
1. Business Operation
   ↓
2. Domain Event Raised (in memory)
   ↓
3. SaveChanges() Called
   ↓
4. Outbox Interceptor Triggered
   ↓
5. Events → Outbox Messages (same transaction)
   ↓
6. Database Commit (atomic)
   ↓
7. Background Job Runs (every 10s)
   ↓
8. Outbox Publisher Processes Pending
   ↓
9. Events Published to RabbitMQ
   ↓
10. Messages Marked as Processed
```
