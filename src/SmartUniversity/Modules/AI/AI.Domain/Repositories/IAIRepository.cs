using SmartUniversity.Modules.AI.Domain.Entities;

namespace SmartUniversity.Modules.AI.Domain.Repositories;

public interface IAIRepository
{
    Task AddAsync(ChatHistory chatHistory);
    Task<IEnumerable<ChatHistory>> GetByUserIdAsync(Guid userId);
    Task<ChatHistory?> GetByIdAsync(Guid id);
    Task DeleteAsync(ChatHistory chatHistory);
}
