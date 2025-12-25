using SmartUniversity.Modules.Identity.Application.Interfaces;
namespace SmartUniversity.Modules.Identity.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }
    }
}
