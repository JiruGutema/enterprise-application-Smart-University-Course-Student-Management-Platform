namespace SmartUniversity.Modules.Identity.Application.DTO
{
    public class LoginRequest
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
