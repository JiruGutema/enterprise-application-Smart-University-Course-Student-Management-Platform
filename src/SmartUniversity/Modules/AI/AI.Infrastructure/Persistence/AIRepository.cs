using Microsoft.EntityFrameworkCore;
using SmartUniversity.Modules.AI.Domain.Entities;
using SmartUniversity.Modules.AI.Domain.Repositories;

namespace SmartUniversity.Modules.AI.Infrastructure.Persistence;

public class AIRepository : IAIRepository
{
    private readonly AIDbContext _context;

    public AIRepository(AIDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ChatHistory chatHistory)
    {
        await _context.ChatHistories.AddAsync(chatHistory);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ChatHistory>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ChatHistories
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChatHistory?> GetByIdAsync(Guid id)
    {
        return await _context.ChatHistories.FindAsync(id);
    }

    public async Task DeleteAsync(ChatHistory chatHistory)
    {
        _context.ChatHistories.Remove(chatHistory);
        // Using ExecuteDeleteAsync can be more efficient but mixing tracking and ExecuteDelete is tricky.
        // Standard remove is fine.
        await _context.SaveChangesAsync();
    }
}
