using SmartUniversity.Modules.AI.Application.DTOS;
using SmartUniversity.Modules.AI.Application.Interfaces;
using SmartUniversity.Modules.AI.Domain.Entities;
using SmartUniversity.Modules.AI.Domain.Repositories;

namespace SmartUniversity.Modules.AI.Application.Services;

public class AIService : IAIService
{
    private readonly IOpenAiService _openAi;
    private readonly IAIRepository _repository;

    public AIService(IOpenAiService openAi, IAIRepository repository)
    {
        _openAi = openAi;
        _repository = repository;
    }

    public async Task<AskAIResponse> AskAIAsync(Guid userId, AskAIRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
             throw new ArgumentException("Prompt cannot be empty");

        var responseText = await _openAi.GenerateContentAsync(request.Prompt);

        var chatHistory = new ChatHistory(userId, request.Prompt, responseText);
        await _repository.AddAsync(chatHistory);

        return new AskAIResponse { Response = responseText };
    }

    public async Task<IEnumerable<ChatHistoryDto>> GetHistoryAsync(Guid userId)
    {
        var history = await _repository.GetByUserIdAsync(userId);
        return history.Select(h => new ChatHistoryDto(h.Id, h.UserPrompt, h.AIResponse, h.CreatedAt));
    }

    public async Task DeleteHistoryAsync(Guid userId, Guid id)
    {
        var chat = await _repository.GetByIdAsync(id);
        if (chat == null) return; // or throw NotFound

        if (chat.UserId != userId)
             throw new UnauthorizedAccessException("Cannot delete history of another user");

        await _repository.DeleteAsync(chat);
    }
}
