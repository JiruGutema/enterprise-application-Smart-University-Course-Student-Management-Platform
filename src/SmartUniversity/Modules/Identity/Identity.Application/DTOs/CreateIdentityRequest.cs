namespace SmartUniversity.Modules.Identity.Application.DTO
{
    public class CreateUserRequest
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string FullName { get; init; } = null!;

    }
}
