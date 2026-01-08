using SmartUniversity.Modules.AI.Application.DTOS;

namespace SmartUniversity.Modules.AI.Application.Interfaces;

public interface IAIService
{
    Task<AskAIResponse> AskAIAsync(Guid userId, AskAIRequest request);
    Task<IEnumerable<ChatHistoryDto>> GetHistoryAsync(Guid userId);
    Task DeleteHistoryAsync(Guid userId, Guid id);
}
