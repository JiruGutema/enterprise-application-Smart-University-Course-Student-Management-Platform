# Notification Module Outbox Event Subscriptions

## Overview

The Notification module has been enhanced to subscribe to outbox events from all bounded contexts in the Smart University system. This enables cross-module communication where the notification module can react to domain events from other modules and send appropriate notifications to users.

## Implementation Summary

### Event Subscriptions Added

#### 1. Identity Module Events (Already Existing)
- ✅ `UserRegisteredEvent` → Welcome email + notification
- ✅ `UserLoggedInEvent` → Login detection email + notification  
- ✅ `PasswordChangedEvent` → Password change confirmation email + notification
- ✅ `ResetPasswordRequestedEvent` → Password reset link email + notification

#### 2. Course Module Events (NEW)
- ✅ `CourseCreatedEvent` → Notifies instructor about successful course creation
- ✅ `CoursePublishedEvent` → Logs event (can be extended to notify relevant users)
- ✅ `CourseDeletedEvent` → Logs event (can be extended to notify affected students/instructors)

#### 3. Enrollment Module Events (NEW)
- ✅ `StudentEnrolledEvent` → Notifies student about successful enrollment
- ✅ `StudentDroppedCourseEvent` → Notifies student about course drop
- ✅ `EnrollmentStatusChangedEvent` → Logs event (requires lookup service for full implementation)

#### 4. Grading Module Events (PLACEHOLDER)
- ✅ `GradingEventHandler` → Placeholder for future grading events like:
  - `AssignmentGradedEvent`
  - `GradeUpdatedEvent`
  - `FeedbackProvidedEvent`

#### 5. AI Module Events (PLACEHOLDER)
- ✅ `AIEventHandler` → Placeholder for future AI events like:
  - `AIAnalysisCompletedEvent`
  - `RecommendationGeneratedEvent`
  - `PersonalizedContentGeneratedEvent`

## Files Created/Modified

### New Event Handlers Created
```
src/SmartUniversity/Modules/Notification/Notification.Application/EventHandlers/
├── UserRegisteredEventHandler.cs (existing)
├── UserLogginInEventHandler.cs (existing)
├── PasswordChangedEventHandler.cs (existing)
├── ResetPasswordRequestedEventHandler.cs (existing)
├── CourseCreatedEventHandler.cs (new)
├── CoursePublishedEventHandler.cs (new)
├── CourseDeletedEventHandler.cs (new)
├── StudentEnrolledEventHandler.cs (new)
├── StudentDroppedCourseEventHandler.cs (new)
├── EnrollmentStatusChangedEventHandler.cs (new)
├── GradingEventHandler.cs (placeholder)
└── AIEventHandler.cs (placeholder)
```

### Outbox Infrastructure (for future use)
```
src/SmartUniversity/Modules/Notification/Notification.Infrastructure/Outbox/
└── OutboxMessage.cs (placeholder for when notification module publishes its own events)
```

### Modified Files
- `src/SmartUniversity/Modules/Notification/NotificationModule.cs`
  - Added imports for Course and Enrollment events
  - Added import for `Notification.Application.EventHandlers` namespace
  - Registered new event handlers in DI container
  - Added event subscriptions in `SubscribeNotificationEvents()` method

### Folder Structure (Following Clean Architecture)
- **Application/EventHandlers/**: Business logic event handlers that consume events from other modules
- **Infrastructure/Outbox/**: Technical outbox infrastructure (for future use when notification module publishes events)

This follows the same pattern as other modules:
- **GradingAndAssessment**: EventHandlers in Application/, Outbox infrastructure in Infrastructure/
- **Identity**: Outbox infrastructure in Infrastructure/Outbox/
- **Courses**: Outbox infrastructure in Infrastructure/Outbox/
- **Enrollment**: Outbox infrastructure in Infrastructure/Outbox/

## Event Handler Patterns

Each event handler follows the established pattern:

```csharp
public class EventHandler
{
    private readonly ILogger<EventHandler> _logger;
    private readonly INotificationServices _notificationServices;

    public async Task HandleAsync(Event evt)
    {
        _logger.LogInformation("Event received...");
        
        // Create notification
        var notification = new Notifications(
            userId,
            title,
            message,
            notificationType
        );
        
        await _notificationServices.CreateNotificationAsync(notification);
    }
}
```

## Outbox Integration

The notification module now subscribes to events through the existing outbox pattern:

1. **Event Generation**: Domain events are raised in other bounded contexts
2. **Outbox Storage**: Events are stored in outbox tables during database transactions
3. **Background Publishing**: Quartz jobs publish events to RabbitMQ every 10 seconds
4. **Event Consumption**: Notification module receives events via RabbitMQ subscriptions
5. **Notification Creation**: Event handlers create appropriate notifications for users

## Event Flow Example

```
Course Module                    Notification Module
     │                                   │
     ▼                                   │
[CourseCreated]                         │
     │                                   │
     ▼                                   │
[OutboxMessage]                         │
     │                                   │
     ▼                                   │
[RabbitMQ Publish] ────────────────────▶│
                                        │
                                        ▼
                              [CourseCreatedEventHandler]
                                        │
                                        ▼
                              [Create Notification]
                                        │
                                        ▼
                              [Notify Instructor]
```

## Benefits

1. **Decoupled Communication**: Modules communicate through events without direct dependencies
2. **Reliable Delivery**: Outbox pattern ensures events are delivered even if services are temporarily unavailable
3. **Scalable**: Event-driven architecture allows for easy addition of new event types and handlers
4. **Auditable**: All events are logged and can be traced through the system
5. **Extensible**: Easy to add new notification types as the system grows

## Future Enhancements

1. **Email Integration**: Extend handlers to send emails for critical events
2. **User Preferences**: Allow users to configure which notifications they want to receive
3. **Notification Templates**: Create reusable templates for different notification types
4. **Push Notifications**: Add support for real-time push notifications
5. **Event Filtering**: Add logic to filter events based on user roles and preferences
6. **Batch Notifications**: Group related notifications to avoid spam

## Configuration

The notification subscriptions are automatically registered when the application starts:

```csharp
// In Program.cs
app.SubscribeNotificationEvents(); // Registers all notification event subscriptions
```

All event handlers are registered in the DI container and will be automatically resolved when events are received.