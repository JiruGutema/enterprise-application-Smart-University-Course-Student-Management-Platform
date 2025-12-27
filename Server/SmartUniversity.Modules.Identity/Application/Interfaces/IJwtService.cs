using SmartUniversity.Modules.Identity.Application.Security;

namespace SmartUniversity.Modules.Identity.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, string email, string role, TokenType tokenType);
        bool ValidateToken(string token, TokenType expectedType, out Guid userId);
    }
}
