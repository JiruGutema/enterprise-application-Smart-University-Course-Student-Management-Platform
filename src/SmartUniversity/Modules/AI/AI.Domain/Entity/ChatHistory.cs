using SmartUniversity.Shared.Kernel;

namespace SmartUniversity.Modules.AI.Domain.Entities;

public class ChatHistory : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string UserPrompt { get; private set; }
    public string AIResponse { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ChatHistory() { }

    public ChatHistory(Guid userId, string userPrompt, string aiResponse)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        UserPrompt = userPrompt;
        AIResponse = aiResponse;
        CreatedAt = DateTime.UtcNow;
    }
}
