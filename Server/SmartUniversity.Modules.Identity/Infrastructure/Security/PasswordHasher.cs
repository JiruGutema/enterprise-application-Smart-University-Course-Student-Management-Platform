using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Infrastructure.Exceptions;

namespace SmartUniversity.Modules.Identity.Infrastructure.Security
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            try
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
            catch (Exception ex)
            {
                throw new PasswordHashException("Failed to hash password", ex);
            }
        }

        public bool Verify(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch (Exception ex)
            {
                throw new PasswordHashException("Failed to verify password", ex);
            }
        }
    }
}
