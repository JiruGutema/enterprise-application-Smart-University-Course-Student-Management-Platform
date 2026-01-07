namespace SmartUniversity.Modules.AI.Application.Interfaces;

public interface IOpenAiService
{
    Task<string> GenerateContentAsync(string prompt);
}
