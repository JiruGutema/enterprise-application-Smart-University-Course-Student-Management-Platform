using OpenAI;
using OpenAI.Chat;
using SmartUniversity.Modules.AI.Application.Interfaces;

namespace SmartUniversity.Modules.AI.Infrastructure.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly OpenAIClient _client;

        public OpenAiService(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAi:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("OpenAI:ApiKey is missing.");

            _client = new OpenAIClient(apiKey);
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            var chatClient = _client.GetChatClient("gpt-4o-mini");

            var response = await chatClient.CompleteChatAsync(
                new ChatMessage[] { new UserChatMessage(prompt) }
            );

            return response.Value.Content[0].Text;
        }
    }
}
