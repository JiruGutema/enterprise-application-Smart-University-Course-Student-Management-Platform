namespace SmartUniversity.Modules.AI.Application.DTOS;

public record ChatHistoryDto(Guid Id, string Prompt, string Response, DateTime CreatedAt);
