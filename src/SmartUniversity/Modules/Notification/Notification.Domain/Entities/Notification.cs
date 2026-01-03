using SmartUniversity.Modules.Notification.Domain.Enums;

namespace SmartUniversity.Modules.Notification.Domain.Entities;

public class Notifications
{
    private Notifications() { }

    public Notifications(Guid userId, string title, string message, NotificationType type)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;

    public NotificationType Type { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    // Domain behavior
    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}
