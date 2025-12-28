using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SmartUniversity.Modules.Identity.Application.Interfaces;
using SmartUniversity.Modules.Identity.Application.Security;

namespace SmartUniversity.Modules.Identity.Infrastructure.Security
{
    public sealed class JwtService : IJwtService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(string secret, string issuer, string audience)
        {
            _secret = secret;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(Guid userId, string email, string role, TokenType tokenType)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(ClaimTypes.Role, role),
                new("token_type", tokenType.ToString()),
            };

            var expires = tokenType switch
            {
                TokenType.Access => DateTime.UtcNow.AddHours(1),
                TokenType.Refresh => DateTime.UtcNow.AddDays(7),
                TokenType.EmailVerification => DateTime.UtcNow.AddHours(24),
                TokenType.PasswordReset => DateTime.UtcNow.AddMinutes(30),
                _ => throw new ArgumentOutOfRangeException(nameof(tokenType)),
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token, TokenType expectedType, out Guid userId)
        {
            userId = Guid.Empty;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            try
            {
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out SecurityToken validatedToken
                );

                var jwt = (JwtSecurityToken)validatedToken;

                var tokenTypeClaim = jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;

                if (
                    tokenTypeClaim is null
                    || !Enum.TryParse(tokenTypeClaim, out TokenType actualType)
                    || actualType != expectedType
                )
                {
                    return false;
                }

                userId = Guid.Parse(
                    jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value
                );

                return true;
            }
            catch
            {
                return false;
            }
        }

        public (string newAccessToken, string newRefreshToken) RefreshAccessToken(
            string refreshToken
        )
        {
            if (!ValidateToken(refreshToken, TokenType.Refresh, out var userId))
                throw new SecurityTokenException("Invalid refresh token");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(refreshToken);

            var email = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            var role = jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value;

            var newAccessToken = GenerateToken(userId, email, role, TokenType.Access);

            var newRefreshToken = GenerateToken(userId, email, role, TokenType.Refresh);

            return (newAccessToken, newRefreshToken);
        }
    }
}
